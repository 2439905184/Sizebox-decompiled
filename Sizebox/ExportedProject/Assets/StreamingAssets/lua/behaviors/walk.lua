Walk = RegisterBehavior("Walk Here")
Walk.scores = {
	normal = 40
}
Walk.data = {
	menuEntry = "Walk/Walk Here",
	ai = true,
	agent = {
		type = { "giantess", "micro" }
	},
	target = {
		type = { "none" }
	},
	tags = "macro, micro, movement",
	settings = {
		{"walkAnim", "Macro Movement Animation", "array", 0, {"Walk", "Walk 2", "Walking", "Walking 2", "Random"}},
		{"lookDownAnim", "Look Down Animation", "string", "random"},
		{"enableAnimMad", "Stomp After Hostile Anim", "bool", true},
		{"enableMacroTarget", "Macro AI: Target Type", "array", 0, {"Micro Only", "Macro Only", "Either"}},
		{"enableWander", "Macro Wander if Same Target Twice", "bool", true},
		{"forceWander", "Macro Force Wander if Other Available Targets", "bool", false},
		{"enableStuckCheck", "Macro Attempt Fix When Stuck", "bool", true},

		{"walkAnimMicro", "Micro Movement Animation", "array", 0, {"Walk", "Walk 2", "Walking", "Walking 2", "Random"}},
		{"enableMacroTargetMicro", "Micro AI: Target Type", "array", 0, {"Micro Only", "Macro Only", "Either"}},
		{"enableWanderMicro", "Micro Wander if Same Target Twice", "bool", true},
		{"forceWanderMicro", "Micro Force Wander if Other Available Targets", "bool", true},
		{"enableStuckCheckMicro", "Micro Attempt Fix When Stuck", "bool", false}
	}
}

walkAnims = {
	{"Walk", 0.8}, {"Walk 2", 0.0001}, {"Walking", 0.000002}, {"Walking 2", 0.00002}
}
idleAnims = {
	"Idle", "Idle 2", "Idle 3", "Idle 4", "Neutral Idle", "Breathing Idle", "Embar", "Thinking"
}
whoopsAnims = {
	"Whatever Gesture", "Look Around", "Look Over Shoulder", "Happy Hand Gesture", "Dismissing Gesture 2", "Defeat", "Look Away Gesture", "Rejected", "Scratch Head"
}
macroAIAnims = {
	"Idle 2", "Idle 5", "Jump", "Jump 3", "Jump 4", "Jump Low", "Look Down", "No", "Pick Up", "Plotting", "Pointing",
	"Whatever Gesture", "Refuse", "Crazy Gesture", "Dismissing Gesture", "Dismissing Gesture 2", "Embar", "Embar 2",
	"Waving", "Waving 2", "Roar", "Pointing Forward", "Scratch Head", "Breathing Idle", "Greet", "Greet 2", "Greet 5",
	"Surprised", "Taunt 3", "Thinking 2", "Victory", "Wait Gesture", "Happy", "Gathering Objects", "Bashful", "Angry", "Wait Strech Arms"
}
microAIAnims = {
	"Greet 2", "Talking", "Talking 2", "Talking 4", "Quick Informal Bow", "No 2", "Whatever Gesture"
}
madAnim = {
	"Angry", "Jump", "Jump 3", "Jump 4", "Plotting", "Happy", "Surprised", "Roar", "Refuse", "Whatever Gesture", "Pointing Forward"
}

macroGapSize = 2.5

function MacroStart(self)

	if self.AION then
		if PrepAITarget(self) then return end
	end

	PrepValues(self)
	WalkToTarget(self)

	if not globals["walkRand"] then math.randomseed(tonumber(os.time()) + self.agent.id) globals["walkRand"] = true end
end

function MacroUpdate(self)
	if not self.targetIsCoords and not self.wanderLostTarget and not (self.arrived and self.arrived == self.arrivalStatus["CASUALTY"]) and (not self.target or self.target.IsDead() or not self.target.IsTargettable()) then
		if not self.target.IsDead() and self.arrived then
			self.arrived = self.arrivalStatus["CASUALTY"]
		else
			TargetGone(self)
			if not self.agent.dict.justWandered then return end
		end
	end

	if self.walking then
		if TestDistance(self,2.5) or not self.agent.ai.IsActionActive() then
			if ArrivedSafely(self) then return end
		else
			if WhileMoving(self) then return end
		end
	end

	if self.AION then
		if AIBehavior(self) then return end
	end
end

microGapSize = 10

function MicroStart(self)

	if self.AION then
		self.isGT = false
		if PrepAITarget(self) then return end
	end

	PrepValues(self)
	WalkToTarget(self)

	if not globals["walkRand"] then math.randomseed(tonumber(os.time()) + self.agent.id) globals["walkRand"] = true end
end

function MicroUpdate(self)
	if not self.targetIsCoords and (not self.target or self.target.IsDead() or not self.target.IsTargettable()) then
		TargetGone(self)
		return
	end

	if self.walking then
		if TestDistance(self,2.5) or not self.agent.ai.IsActionActive() then
			if ArrivedSafely(self) then return end
		else
			if WhileMoving(self) then return end
		end
	end

	if self.AION then
		if AIBehavior(self) then return end
	end
end

function WalkExit(self)
	if self.AION then
		self.agent.dict.previousTarget = self.target
	else
		SetExitAnim(self)
	end
	self.agent.movement.speed = 0.8
end

function Walk:Start()

	self.AION = self.agent.ai.IsAIEnabled()
	self.isGT = self.agent.isGiantess()

	self._exit = WalkExit

	if self.isGT then
		self._update = MacroUpdate
		MacroStart(self)
	else
		self._update = MicroUpdate
		MicroStart(self)
	end
end

function Walk:Update()
	self._update(self)
end

function Walk:Exit()
	self._exit(self)
end


function PrepValues(ref)
	PrepBMSettings(ref)
	if ref.AION then
		if ref.isGT then
			ref.arrivalStatus = {["ARRIVING"] = 0, ["NOCASUALTY"] = 1, ["CASUALTY"] = 2}
			ref.arrived = false
			if ref.lookDownAnim ~= "random" then
				local animList = ref.agent.animation.GetAnimationList()
				for k,v in pairs(animList) do if v == ref.lookDownAnim then animExists = true end end
				if not animExists then log("Walk Here: Chosen animation in behavior manager does not exist.") ref.lookDownAnim = "random" end
			end
		end
	else
		CoordsTargetCheck(ref)
	end

	if ref.enableStuckCheck then
		if ref.isGT then
			ref.timerScaleMin = 8
		else
			ref.timerScaleMin = 3
		end
		ref.lastScale = ref.agent.scale
		ref.lastPosition = ref.agent.transform.position
		ref.timerScaleAdd = Mathf.Clamp(ref.lastScale * 0.06, ref.timerScaleMin, 16)
		ref.timer = Time.time
	end
end

function PrepBMSettings(ref)
	if not ref.isGT then
		ref.walkAnim = ref.walkAnimMicro
		ref.lookDownAnim = nil
		ref.enableAnimMad = nil
		ref.enableMacroTarget = ref.enableMacroTargetMicro
		ref.enableWander = enableWanderMicro
		ref.forceWander = forceWanderMicro
		ref.enableStuckCheck = enableStuckCheckMicro
	end
	ref.walkAnimMicro = nil
	ref.enableMacroTargetMicro = nil
	ref.enableWanderMicro = nil
	ref.forceWanderMicro = nil
	ref.enableStuckCheckMicro = nil
end

function CoordsTargetCheck(ref)
	if not ref.target or ref.target.name == "City" then
		ref.target = ref.cursorPoint
		ref.targetIsCoords = true
	end
end

function WalkToTarget(ref)
	ref.walking = true
	if not ref.targetIsCoords then
		ref.agent.LookAt(ref.target)
		ref.agent.dict.justWandered = false
	else
		ref.agent.LookAt(nil)
	end
	PrepMovementAnim(ref)
	ref.agent.animation.Set(ref.walkAnim)
	ref.agent.MoveTo(ref.target)
end

function PrepAITarget(ref)
	if WantsMacroTarget(ref,ref.enableMacroTarget) then
		ref.target = ref.agent.FindClosestGiantess()
		ref.AIWantsGT = true
	else
		ref.target = ref.agent.FindClosestMicro()
	end
	if not ref.target or ref.target.IsDead() or not ref.target.IsTargettable() then
		ref.agent.ai.StopBehavior()
		return true
	end
	if ref.target == ref.agent.dict.previousTarget then
		if DecideOnWander(ref) then
			WanderAround(ref)
			return true
		end
	end
end

function PrepMovementAnim(ref)
	if ref.walkAnim == #walkAnims then
		ref.walkAnim = math.random(#walkAnims)
	else
		ref.walkAnim = ref.walkAnim + 1
	end
	ref.agent.movement.speed = walkAnims[ref.walkAnim][2]
	ref.walkAnim = walkAnims[ref.walkAnim][1]
end

function TestDistance(ref, mult)
	local separation
	local targetVector
	if ref.targetIsCoords then
		separation = ref.agent.scale * 0.2
		targetVector = ref.target - ref.agent.position
	else
		local smallest = Mathf.Min(ref.agent.scale, ref.target.scale)
		local biggest = Mathf.Max(ref.agent.scale, ref.target.scale)
		local gapSize
		if ref.isGT then gapSize = macroGapSize else gapSize = microGapSize end
		separation = (biggest + smallest * gapSize) * 0.1
		local tx = ref.target.position.x - ref.agent.position.x
		local tz = ref.target.position.z - ref.agent.position.z
		targetVector = Vector3.New(tx,0,tz)
	end
	return targetVector.magnitude < separation * mult
end

function TargetGone(ref)
	if ref.checkedAvailableTargets then
		if ref.agent.dict.justWandered or (ref.exitAnimTimer and ref.exitAnimTimer > Time.time) then return end
		ref.agent.ai.StopBehavior()
	else
		if NoPotentialTargets(ref) then
			SetExitAnim(ref)
			ref.exitAnimTimer = Time.time + 2
		elseif ref.agent.dict.justWandered then
			ref.wanderLostTarget = true
		end
		ref.checkedAvailableTargets = true
	end
end

function SetExitAnim(ref)
	local idleAnim = idleAnims[math.random(#idleAnims)]
	ref.agent.animation.Set(idleAnim)
end

function isAnimMad(anim)
	for k,v in pairs(madAnim) do
		if v == anim then return true end
	end
end

function WantsMacroTarget(ref,setting)
	local macroTarget
	if setting > 0 and ref.agent.GetRandomGiantess() then
		if setting == 2 then
			macroTarget = 2 == math.random(2)
		else
			macroTarget = true
		end
	end
	return macroTarget
end

function DecideOnWander(ref)
	local targetList,many = GetPotentialTargets(ref)
	if many then
		if ref.forceWander and not ref.agent.dict.justWandered then
			return true
		else
			ref.target = ChooseDiffTarget(ref,targetList,0.5,0.5)
			if ref.agent.dict.wanderCounter then ref.agent.dict.wanderCounter = 0 end
		end
	else
		if ref.enableWander and PrepWander(ref) then
			return true
		end
	end
end

function GetPotentialTargets(ref)
	local targetList
	local many
	for r=1,2,1 do
		targetList = MicroOrMacroList(ref,r)
		many = #targetList >= 2
		if many then break end
	end
	return targetList,many
end

function MicroOrMacroList(ref,r)
	local list
	local radius = Mathf.Max(ref.agent.scale,50) * 5 * r
	if ref.AIWantsGT then
		list = ref.agent.senses.GetGiantessesInRadius(radius)
	else
		list = ref.agent.senses.GetMicrosInRadius(radius)
	end
	return list
end

function NoPotentialTargets(ref)
	local targetList = GetPotentialTargets(ref)
	return #targetList == 0
end

function ChooseDiffTarget(ref,list,pos,adjust)
	if not adjust then adjust = 0 end
	local tablePos = math.ceil(#list * pos + adjust)
	local t = list[tablePos]
	if t.id == ref.target.id then
		t = list[tablePos-1]
	end
	return t
end

function PrepWander(ref)
	if not ref.agent.dict.wanderCounter then ref.agent.dict.wanderCounter = 0 end
	ref.agent.dict.wanderCounter = ref.agent.dict.wanderCounter + 1
	if ref.agent.dict.wanderCounter > 3 then
		if ref.agent.dict.wanderCounter == 10 then ref.agent.dict.wanderCounter = 0 end
		return false
	else
		return true
	end
end

function WanderAround(ref)
	local duration = math.random(5, 8)
	ref.stop = true
	ref.agent.dict.justWandered = true
	ref.agent.LookAt(nil)
	PrepMovementAnim(ref)
	ref.agent.animation.Set(ref.walkAnim)
	ref.agent.wander(duration)
end

function StuckCheck(ref)
	if ref.checking then
		if Time.time > ref.timer then
			ref.distanceCheck = ref.agent.DistanceTo(ref.lastPosition) < ref.agent.scale * 0.5
			UpdateDistanceMarkers(ref)
			if ref.distanceCheck then
				if StuckFix(ref) then return true end
			else
				ref.timer = Time.time + ref.timerScaleAdd
			end
		end
	else
		UpdateDistanceMarkers(ref)
		ref.timer = Time.time + ref.timerScaleAdd
		ref.checking = true
	end
end

function UpdateDistanceMarkers(ref)
	ref.lastPosition = ref.agent.transform.position
	if ref.agent.scale ~= ref.lastScale then
		ref.lastScale = ref.agent.scale
		ref.timerScaleAdd = Mathf.Clamp(ref.lastScale * 0.06, ref.timerScaleMin, 16)
	end
end

function StuckFix(ref)
	ref.agent.ai.StopBehavior()
	return true
end

function ArrivedSafely(ref)
	if ref.AION then
		if ref.isGT then ref.arrived = ref.arrivalStatus["NOCASUALTY"] end
		ref.agent.ai.StopAction()
	else
		ref.agent.ai.StopBehavior()
		return true
	end
	ref.walking = false
end

function WhileMoving(ref)
	if ref.enableStuckCheck then
		if StuckCheck(ref) then return true end
	end
end

function AIBehavior(ref)
	if ref.isGT and ref.walking and not ref.arrived and TestDistance(ref,5) then
		ref.arrived = ref.arrivalStatus["ARRIVING"]
	end
	if not ref.agent.ai.IsActionActive() then
		if ref.stop then
			if ref.woopsie then
				TargetGone(ref)
			else
				ref.agent.ai.StopBehavior()
			end
			return true
		end
		if PlayAIAnims(ref) then return true end
	end
end

function PlayAIAnims(ref)
	if ref.isGT then
		if not ref.lookedDown then
			if PlayLookDownAnim(ref) then return true end
		else
			if not ref.AIWantsGT and ref.enableAnimMad and isAnimMad(ref.lookDownAnim) then
				AngryMacro(ref)
			else
				ref.stop = true
			end
		end
	else
		if not ref.microTalked then
			if PlayMicroAnim(ref) then return true end
		else
			ref.stop = true
		end
	end
end

function PlayMicroAnim(ref)
	ref.microAnim = microAIAnims[math.random(#microAIAnims)]
	ref.agent.animation.SetAndWait(ref.microAnim)
	ref.microTalked = true
end

function PlayLookDownAnim(ref)
	if ref.arrived and (not ref.target or ref.target.IsDead() or not ref.target.IsTargettable()) then
		ref.agent.LookAt(nil)
		local whoopsAnim = whoopsAnims[math.random(#whoopsAnims)]
		ref.agent.animation.SetAndWait(whoopsAnim)
		ref.woopsie = true
		ref.stop = true
		return true
	end
	if ref.lookDownAnim == "random" then
		ref.lookDownAnim = macroAIAnims[math.random(#macroAIAnims)]
	end
	ref.agent.animation.SetAndWait(ref.lookDownAnim)
	ref.lookedDown = true
end

function AngryMacro(ref)
	if not ref.stomped then
		ref.agent.animation.SetAndWait("Stomping")
		ref.stomped = true
	else
		if not ref.target or ref.target.IsDead() or not ref.target.IsTargettable() then
			ref.target = ref.agent.findClosestMicro()
		end
		if not ref.target then ref.stop = true return end
		ref.agent.dict.ai = true
		globals["AIWalkStomp"] = {[ref.agent.id] = true}
		ref.agent.ai.SetBehavior("Stomp",ref.target)
	end
end

function LookAtMe(ref,toggle)
	if toggle ~= ref.looking then
		ref.looking = toggle
		local lookee
		if toggle then
			lookee = ref.agent
		else
			lookee = nil
		end
		ref.target.LookAt(lookee)
	end
end
