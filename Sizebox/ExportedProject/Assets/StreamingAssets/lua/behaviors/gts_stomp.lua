Stomp = RegisterBehavior("Stomp")
Stomp.scores = {
	normal = 30
}
Stomp.data = {
	menuEntry = "Stomp",
	ai = true,
	agent = {
		type = { "giantess" }
	},
	target = {
		type = { "micro" }
	},
	tags = "macro, movement, evil",
	settings = {
		{"limitDistance", "AI: Only target nearby micros", "bool", false}, --when using in AI, don't allow choosing a micro at the other end of the world when scanning
		{"mercifulMode", "Merciful Mode", "bool", false}, --spare micros after failed stomp attempt
		{"mercyLevels", "3-Strike Sparing", "bool", false}, --tag spared micros for sparing up to 3 times
		{"mercifulWander", "Wander if Merciful", "bool", false}, --should wander if no other micro found?
		{"enableStuckCheck", "Attempt Fix When Stuck", "bool", false},
		{"nonAIStuckCheck", "Stuck fix without AI", "bool", false} --enable checking for 'stuck when walking' also outside of AI
	}
}

idleAnims = {"Idle 2", "Idle 4", "Neutral Idle", "Crouch Idle", "Greet", "Breathing Idle", "Bored", "Wait Torso Twist", "Idle 5", "Greet 2", "Rejected", "Bashful"}
angryAnims = {"Stomping", "Jump 4", "Angry"}
wanderEndAnims = {"Idle 5", "Embar", "Embar 2", "Bored", "Look Around", "No 2", "Refuse", "Relieved Sigh", "Scratch Head", "Stomping", "Thinking", "Thinking 2", "Wait Stretch Arms", "Yawn", --less expressive anims (1-14)
				  "Jump Low", "Jump 4", "Greet 4", "Disappointed", "Defeat", "Look Over Shoulder", "Rejected", "Shake Fist", "Wait Torso Twist", "Waving 2", "Whatever Gesture" } --more expressive anims (15-25)
wanderPartingAnims = {"Greet 2", "Dismissing Gesture", "Dismissing Gesture 2"}
lookDownAnim = "Greet"
walkAnimation = "Walk"
idleAnimation = "Idle 2"
noTargetTransitionAnimation = "Neutral Idle"
exitIdleAnimation = "Idle 4"
WAITING, STOMPING, WALKING, AVOIDING = 0, 1, 2, 3

stompAnimModule = require "stomp_anim"

function Stomp:Start()
	--TODO: Simplify this
	--If AI
	self.AIWalkStomp = globals["AIWalkStomp"] and globals["AIWalkStomp"][self.agent.id]
	self.AION = self.agent.ai.IsAIEnabled() or self.AIWalkStomp

	if self.AION then
		--If behavior started from AI Walk, set AI ON, and remove item from global table. Delete global table if empty
		if self.AIWalkStomp then
			globals["AIWalkStomp"][self.agent.id] = nil
			if #globals["AIWalkStomp"] == 0 then globals["AIWalkStomp"] = nil end
		end
		self.AIWalkStomp = nil
		self.mercifulMode = false

		if not self.target then self.target = self.agent.GetRandomMicro() end

		--If target chosen by AI is too far, choose closest one
		if self.limitDistance and self.target and self.agent.DistanceTo(self.target) > self.agent.scale * 4 then
			self.target = self.agent.FindClosestMicro()
		end

		--It's pointless to continue without a target to stomp, so let's make sure we have one
		if not self.target then self.targetGone = true self.agent.ai.StopBehavior() return end
	end

	--prep mercy
	if self.mercifulMode then
		self.stompFunc = MercyStomp
	else
		self.stompFunc = NormalStomp
	end

	--prep for stuck check
	self.lastScale = self.agent.scale
	self.lastPosition = self.agent.transform.position
	self.timerScaleAdd = Mathf.Clamp(8 + self.lastScale * 0.06, 7, 16)

	--prep for 1st block
	self:WaitState(-1)

	--only run randomseed once per scene, per script
	if not globals["StompRand"] then math.randomseed(tonumber(os.time()) + self.agent.id) globals["StompRand"] = true end
end

function Stomp:Update()
	--If WAITING and TIMER is out
	if self.state == WAITING then
		if self.timer < Time.time then

			--If target=nil or target=false OR target=notTargettable
			if not self.target or self.target.IsDead() or not self.target.IsTargettable() then
				self:FindATarget()
				return

			--If valid target found, see if you can stomp it, and move closer if not
			else
				if self.enableStuckCheck then --reset this to prep for stuckCheck
					self.checking = false
				end
				self:WalkState()
			end
		end

	--If WALKING or about to
	elseif self.state == WALKING then

		--Check for dead/missing target
		if not self.target or self.target.IsDead() or not self.target.IsTargettable() then
			self:DeadTargetCheck()
			return
		end

		local separation = (self.agent.scale + self.target.scale) * 0.2
		local targetVector = self.target.position - self.agent.position

		--When arrived at target: Stomp it
		if targetVector.magnitude < separation * 1.4 then
			self:StompState()

		--If not moving: MoveTo target
		elseif not self.agent.ai.IsActionActive() then
			self:WalkToTarget()

		--If moving, and if StuckCheck enabled, check if entity gets stuck while walking; if it gets stuck, stop behavior.
		elseif self.enableStuckCheck then
			self:StuckCheck()
		end

	--If STOMPING (or AVOIDING if mercy)
	else
		--Live LookAt Switcher (make target look at me if Stomping or Avoiding, and close enough, and target exists)
		if self.target and self.target.IsTargettable() then
			local ifNear = self.agent.DistanceTo(self.target) < self.agent.scale * 2
			self:LookAtMe(ifNear)
		end

		--Run stomp block (according to mercifulMode)
		self.stompFunc(self)
	end
end

function Stomp:Exit()
	--if not in AI, OR in AI and no more target, play an animation in case next AI behavior doesnt set an anim right away
	if (not self.agent.ai.IsAIEnabled() or self.targetGone) then
		self.agent.animation.Set(exitIdleAnimation)
	end
end



--[[ FUNCTIONS ]]--

function NormalStomp(self)
	--If in STOMP action
	if self.state == STOMPING then

		--When Stomp action finished (foot has stomped the ground and moved back into position)
		if not self.agent.ai.IsActionActive() then
			if self.AION then
				self:LookAtMe(false)
				self.agent.ai.StopBehavior()
				return
			end
			self:WaitState(2)
		end
	end
end

function MercyStomp(self)
	--If in STOMP action
	if self.state == STOMPING then

		--When Stomp action finished (foot has stomped the ground and moved back into position)
		if not self.agent.ai.IsActionActive() then

			--If target survived, and target is not at spared lvl 4
			if not self.target.IsDead() and self.target.IsTargettable() and not self.target.dict.noMercy then

				--Run Spare branching options
				self:Spare()

			--If target survived and was spared 3 times OR target died
			--set timer and reset to base loop (if target is still alive, it hasn't changed. Base loop will directly lead to another Stomp() attempt)
			else
				self:WaitState(2)
			end
		end

	--If Avoiding target by wandering or idling still
	--AND if Wandering: If done wandering. OR if Idling: If idle anim has either fully played, or 8s timer passed.
	elseif (not self.agent.ai.IsActionActive()) or (not self.mercifulWander and self.timer < Time.time) then

		--After wander intro anim played, do actual Wander()
		if self.wanderLookDown then
			self:GoWander(10)
			self.wanderLookDown = nil
			self.wandering = true

		elseif self.wandering then
			self:WanderEndAnim()
			self.wandering = nil

		--if Idling AND 1st anim finished before 8s timer (or keeping mercy simple), play one more anim
		elseif (not self.mercyLevels or not self.mercifulWander) and self.avoidAnim and self.timer > Time.time then
			self:AvoidIdle()

		--if done wandering or have played idles, loop right back into choosing target
		else
			--reset a couple temp variables
			if self.avoidAnim then
				self.avoidAnim = nil
				if self.transD then
					self.agent.animation.transitionDuration = self.transD
					self.transD = nil
				end
				--stop anims from preventing next loop (actionActive)
				self.agent.ai.StopAction()
			end

			--check if new targets appeared after 'avoiding'
			local micros,many = self:GetPotentialTargets()

			--set one of them as the target, if any
			if many then
				self:LookAtMe(false)
				self.target = self:ChooseDiffTarget(micros,0.5,0.5)
			end
			self:WaitState(0)
		end
	end
end

function Stomp:FindATarget()
	--if target not valid but still targeted somehow
	--remove from spared list and disable looking at me
	if self.target and (self.target.IsDead() or not self.target.IsTargettable()) then
		if self.sparedList then self.sparedList[self.target.id] = nil self.target.dict.noMercy = nil end
		self:LookAtMe(false)
	end

	--set idle animation for waiting if not already playing
	self.agent.animation.Set(idleAnimation)

	--look for a new target
	self.target = self.agent.FindClosestMicro()

	--if target found, re-run this block immediately
	if self.target then
		self.timer = Time.time

	--if not, wait 1s before trying again
	else
		self.timer = Time.time + 1
	end

end


function Stomp:WalkToTarget()
	--only set walk animation now, in case agent is already within stomping range
	self.agent.animation.Set(walkAnimation)
	self.agent.MoveTo(self.target)

	--Roll random fear anim on target micro when agent starts walking
	if self.target.ai.IsAIEnabled() and not self.target.isPlayer() then
		self.target.ai.SetBehavior("Fear")
	end
end


function Stomp:Spare()
	if self.mercyLevels then
		self:UpdateSpareLvl()

		--if raised to lvl 4 just now, mark as 'noMercy' and don't choose a mercy act, skip straight back to Stomp()
		if self.sparedList[self.target.id] == 4 then
			self.target.dict.noMercy = true
			self:WaitState(2)
			return
		end
	end

	self:MercyActChooser()
end


function Stomp:UpdateSpareLvl()
	--create spared micro list if first time
	if not self.sparedList then self.sparedList = {} end

	--if micro has never been spared/tagged, add to the list and mark as lvl 1
	if not self.sparedList[self.target.id] then
		self.sparedList[self.target.id] = 1

	--if micro is known by GTS, increase spare lvl
	else
		self.sparedList[self.target.id] = self.sparedList[self.target.id] + 1
	end
end


function Stomp:MercyActChooser()
	--if spare lvl 1 or 2 OR no mercyLevels
	if not self.mercyLevels or self.sparedList[self.target.id] < 3 then

		--if 2nd time sparing, play angry stomp animation
		if self.mercyLevels and self.sparedList[self.target.id] == 2 then
			self.agent.animation.SetAndWait(angryAnims[1])
		end

		--Depending on available targets (1 vs more), choose mercy action
		self:DecideOnTarget()

	--if spare lvl 3
	else
		self.agent.animation.SetAndWait(angryAnims[2])
		self.agent.animation.SetAndWait(angryAnims[1])
		self.state = AVOIDING
		self.timer = Time.time + 30
	end
end


function Stomp:DecideOnTarget()
	--fetch list of targetable micros
	local micros,many = self:GetPotentialTargets()

	--if at least one other micro around
	if many then
		self:LookAtMe(false)
		self.target = self:ChooseDiffTarget(micros,0.5,0.5)
		self:WaitState(2)

	--if current target is the only micro available around, choose Avoid action
	else
		self:MercyAvoid()
	end
end


function Stomp:MercyAvoid()
	--if wander enabled, wander a bit
	if self.mercyLevels and self.mercifulWander then
		self:MercyWander()

	--if wander not enabled, play two idle anims and try again
	else
		self:MercyIdle()
	end
	self.state = AVOIDING
end


function Stomp:MercyWander()
	--if gts head high above target enough, quickly look down before wandering off
	local base = self.agent.position.y --vertical coord of bottom of GTS
	local aHeight = self.agent.bones.head.position.y - base --topOfGTSHead - base = GTS height
	local tHeight = self.target.bones.head.position.y - base --topOfMicroHead - base = vertical distance to bottom of GTS
	if tHeight <= aHeight * 0.5 then --if micro head is vertically under half GTS height... GTS looks down.
		self.agent.animation.SetAndWait(lookDownAnim)

	--else, play anim better suited to size ratio
	else
		local anim = wanderPartingAnims[math.random(#wanderPartingAnims)]
		self.agent.animation.SetAndWait(anim)
	end

	self.wanderLookDown = true
end


function Stomp:WanderEndAnim()
	--roll for 1/4 chance of a more expressive anim (make these less common)
	local i = math.random(4)
	if i == 4 then
		i = 14 + math.random(11)
	else
		i = math.random(14)
	end
	self.agent.animation.SetAndWait(wanderEndAnims[i])
end


function Stomp:MercyIdle()
	self.avoidAnim = idleAnims[math.random(#idleAnims)]

	--prep for potential Crouch Idle
	if not self.transD then self.transD = self.agent.animation.transitionDuration end

	--if keeping mercy simple, play only 1 idle animation
	if not self.mercyLevels then self.timer = Time.time + 1 return end

	--if Crouch Idle was rolled, increase animation transition duration, default is too fast
	if self.avoidAnim == "Crouch Idle" then
		local factor = 20 / self.agent.animation.GetLength()
		self.agent.animation.transitionDuration = self.transD * factor
	end

	self.agent.animation.SetAndWait(self.avoidAnim)
	self.timer = Time.time + 8
end


function Stomp:AvoidIdle()
	--roll for chance of "Angry" idle anim
	local anger = 4 == math.random(4)

	if anger then
		self.avoidAnim = angryAnims[3]
	else
		self.avoidAnim = idleAnims[math.random(#idleAnims)]
		if self.avoidAnim == "Crouch Idle" then
			local factor = 20 / self.agent.animation.GetLength()
			self.agent.animation.transitionDuration = self.transD * factor
		else
			--restore default value because no more Crouch Idle this loop
			self.agent.animation.transitionDuration = self.transD
		end
	end

	--after choosing, play 2nd idle anim for at most 8s
	self.agent.ai.StopAction()
	self.agent.animation.SetAndWait(self.avoidAnim)
	self.timer = Time.time + 8
end


function Stomp:DeadTargetCheck()
	--if missing target during WALK (don't want to hard cut Stomp() + WAIT & AVOID don't need a missing target check)
	--If AI, skip to next behavior
	if self.AION or self.agent.ai.HasQueuedBehaviors() then
		self.targetGone = true
		self.agent.ai.StopBehavior()

	--if not AI, stop any action; this restarts the target seeking loop in WAITING block
	else
		self.agent.ai.StopAction()
		self.agent.animation.Set(noTargetTransitionAnimation)
		self:WaitState(2)
	end
	self.agent.LookAt(nil)
	self.target.LookAt(nil)
end


function Stomp:StuckCheck()
	--markers are set, if timer has run
	if self.checking then
		if Time.time > self.timer then

			--Test live value against marker
			self.distanceCheck = self.agent.DistanceTo(self.lastPosition) < self.agent.scale * 0.5

			--Update markers for next check
			self:UpdateDistanceMarkers()

			--if either check was successful: Agent is stuck
			if self.distanceCheck then
				self:StuckFix()

			--else: Agent is not stuck. Reset timer for another check
			else
				self.timer = Time.time + self.timerScaleAdd
			end
		end
	else
		--prep for 1st check
		self:UpdateDistanceMarkers()
		self.timer = Time.time + self.timerScaleAdd
		self.checking = true
	end
end


function Stomp:UpdateDistanceMarkers()
	self.lastPosition = self.agent.transform.position

	--Reset lastScale only if agent height has changed, and adjust timer modifier accordingly
	if self.agent.scale ~= self.lastScale then
		self.lastScale = self.agent.scale
		self.timerScaleAdd = Mathf.Clamp(8 + self.lastScale * 0.06, 7, 16)
	end
end


function Stomp:StuckFix()
	--if using AI, skip to next AI behavior
	if self.AION then
		self.agent.ai.StopBehavior()
		return

	--if using "Stuck fix without AI", attempt to get unstuck by seeking a different target
	elseif self.nonAIStuckCheck then
		local micros,many = self:GetPotentialTargets()
		if many then
			self:LookAtMe(false)
			self.agent.ai.StopAction()

			--roll random index to choose target from
			local i = math.random(100) * 0.01
			self.target = self:ChooseDiffTarget(micros,i)
			self:WaitState(0)

		--no alternative, delay next check
		else
			self.timer = Time.time + self.timerScaleAdd
		end
	end
end


--Always toggle this off right before switching to a new target ]]
function Stomp:LookAtMe(toggle)
	if toggle then
		if not self.looking then self.target.LookAt(self.agent) self.looking = true end
	else
		if self.looking then self.target.LookAt(nil) self.looking = false end
	end
end


--Get list of surrounding, targetable micros, and check if more than one
function Stomp:GetPotentialTargets()
	local micros
	local many
	local r = 1
	while not many and r < 3 do --scan for micros in 1x radius, if none found, scan once more and expand search to 5x radius
		micros = self.agent.senses.GetMicrosInRadius(5 * r)
		many = #micros >= 2
		r = r + 1
	end
	return micros,many
end


--Runs if GetPotentialTargets returned a list with more than one micro
function Stomp:ChooseDiffTarget(list,pos,adjust)
	if not adjust then adjust = 0 end					--adjust only used for a certain situation, if its value is 0.5, paired with pos=0.5
	local tablePos = math.ceil(#list * pos + adjust)	--the math ensures to always choose middle index, or index #2 in the case of #list=2
	local t = list[tablePos]	--candidate target micro
	if t.id == self.target.id then	--if chosen micro is current target,
		t = list[tablePos-1]		--choose micro 1 index lower in the list
	end
	return t
end


function Stomp:WaitState(s)
	self.state = WAITING
	self.timer = Time.time + s
end


function Stomp:WalkState()
	self.agent.LookAt(self.target)
	self.state = WALKING
	self.timer = Time.time
end


function Stomp:StompState()
	self.agent.ai.StopAction()
	self.agent.animation.Set(stompAnimModule.getRandomStompAnim())
	self.agent.Stomp(self.target)
	self.state = STOMPING
end


function Stomp:GoWander(s)
	self.agent.animation.Set(walkAnimation)
	self.agent.Wander(s)
end





--[[ COMPANION BEHAVIOR ]]--
-- Here i'm making a custom reaction to this behavior
-- the micro will be scared for at least 10 seconds
MicroFear = RegisterBehavior("Fear")
MicroFear.react = true -- i just mark it as react so is not interrupted by the micro running
MicroFear.data = {
	hideMenu = true,
	agent = {
		type = { "micro"},
		exclude = {"player"}
	},
	target = {
		type = {"oneself"}
	}
}

fearAnim = "Nervously Look Around"

function MicroFear:Start()
	--Only do randomseed once per scene
	if not globals["FearRand"] then math.randomseed(tonumber(os.time()) + self.agent.id) globals["FearRand"] = true end
	self.roll = math.random(1,10)
	self.fearDuration = math.random(5,10)
	if self.roll < 5 then
		self.agent.ai.StopBehavior()
	else
		self.agent.animation.Set(fearAnim)
		self.startTime = Time.time + self.fearDuration
	end
end

function MicroFear:LazyUpdate()
	if self.startTime and Time.time > self.startTime  then
		self.agent.ai.StopBehavior()
	end
end
