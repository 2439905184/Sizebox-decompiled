Raygun = RegisterBehavior("raygun_example")
Raygun.data =  {
    menuEntry = "Raygun/Utility Example",
    secondary = true,
	flags = {"raygun"},
    agent = {
        type = { "player" }
    },
    target = {
        type = { "oneself" }
    },
	settings = {
		{ "walkAnimation", "walking animation", "string", "Walk" },
		{ "idleAnimation", "idle animation", "string", "Idle 2" },
		{ "enrage_mult", "enrage multiplier", "float", 0.5 },
		{ "stop_key", "Stop key", "keybind", "k" }
	}
}




-- OnRaygunHit event will pass the following data to this function:
-- target - entity that has been hit
-- magnitude - raygun's magnitude value (float | ranges from -1 to 1)
-- firingMode - type of emission that hit target (int | 0 - projectile, 1 - laser, 2 - sonic)
-- chargeValue - value of hit projectile charge value (float | ranges from 0.125 - 1)

function Raygun:Listener(data)
    -- sonic mode causes GTSs and micros to follow you
	if data.firingMode == 0 then 
		if data.target.isGiantess() then
			data.target.dict.killCount = Mathf.Round(data.chargeValue * self.enrage_mult * 10)
			data.target.ai.SetBehavior("EnragedGTS")
		end
	elseif data.firingMode == 1 then
		if data.target.isGiantess() then
			self.laserCrusher = data.target
		elseif data.target.isMicro() then
			self.laserTarget = data.target
			if self.laserCrusher ~= nil then
				self.laserCrusher.lookAt(self.laserTarget)
				self.laserCrusher.animation.Set(self.walkAnimation)
				self.laserCrusher.moveTo(self.laserTarget)
				self.laserCrusher.animation.Set(self.idleAnimation)
				self.laserCrusher.Stomp(self.laserTarget)
			end
		end
	elseif data.firingMode == 2 then 
		if not data.target.dict.IsFollowing then
			data.target.LookAt(self.agent)
			data.target.animation.Set(self.walkAnimation)
			data.target.Seek(self.agent, 0, -1)
			data.target.dict.IsFollowing = true
			
			table.insert(followers, data.target)
		end
	end
end

function Raygun:Start()
	followers = {}
	self.laserTarget = nil
	self.laserCrusher = nil

    -- subscribe Listener() to a "OnRaygunHit" event
    self.agent.dict.OnRaygunHit = Event.Register(self, EventCode.OnPlayerRaygunHit, self.Listener)
end

function Raygun:Update()
	if Input.GetKeyDown(self.stop_key) then
		self.agent.ai.StopSecondaryBehavior("raygun")
	end
end

function Raygun:Exit()
	for key, follower in pairs(followers) do
		follower.dict.IsFollowing = nil
		follower.ai.StopAction()
		follower.animation.Set(self.idleAnimation)
	end
end

EnragedGTS = RegisterBehavior("EnragedGTS")
EnragedGTS.react = true -- i just mark it as react so is not interrupted by the micro running
EnragedGTS.data = {
    hideMenu = true,
    agent = {
        type = {"giantess"}, 
        exclude = {"player"}
    },
    target = {
        type = {"oneself"}
    }
}

walkAnimation = "Walk"
idleAnimation = "Idle 2"

function EnragedGTS:Start()
    self.agent.ai.StopAction()
	self.toStompCounter = self.agent.dict.killCount
	
	self.target = self.agent.findClosestMicro()
	if not self.target then
		self.agent.ai.StopBehavior()
		return
	end
end 

function EnragedGTS:Update()
	if not self.agent.ai.IsActionActive() then
		-- if current target is crushed, decrement to-stomp counter and find new target
		if not self.target or not self.target.IsTargettable() then		
			self.toStompCounter = self.toStompCounter - 1
			self.target = self.agent.findClosestMicro()
            if not self.target or self.toStompCounter <= 0 then
                self.agent.ai.StopBehavior()
                return
            end
        end

        self.agent.lookAt(self.target)
        self.agent.animation.Set(walkAnimation)
        self.agent.moveTo(self.target)
        self.agent.animation.Set(idleAnimation)
        self.agent.Stomp(self.target)
	end
end

function EnragedGTS:Exit()
    self.agent.animation.Set(idleAnimation)
end