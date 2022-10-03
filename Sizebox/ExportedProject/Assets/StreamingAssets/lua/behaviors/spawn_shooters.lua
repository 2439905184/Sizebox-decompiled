--common micro spawning function

function SpawnMicros(self, microCount, targetEntity, spawn_callback)
	micros = {}

	models = {}
    models[true] = Entity.GetFemaleMicroList()
    models[false] = Entity.GetMaleMicroList()
	local female = true
	local pos = targetEntity.position + Vector3.New(0, 3, 0)
    local angle = targetEntity.transform.rotation.eulerAngles.y + 180
	
	for i=1,microCount do
        local model = models[female][math.random(#models[female])]
        local microAngle = math.random(360)
        local microRot = Quaternion.angleAxis(microAngle, Vector3.up)
        local microPos = pos - microRot * Vector3.forward * math.random(50, 100)
        
		if female then
			micro = Entity.SpawnFemaleMicro(model, microPos, microRot, 1, self, spawn_callback)
		else
			micro = Entity.SpawnMaleMicro(model, microPos, microRot, 1, self, spawn_callback)
		end
		
		female = not female
		
		if micro ~= nil then
			micro.ai.DisableAI()
			table.insert(micros, micro)
		end
    end

	return micros
end

----------------------------------------------------------



SGGS = RegisterBehavior("SpawnGrowSqaud")
SGGS.data = {
    menuEntry = "Spawn/Grow Squad",
    secondary = true,
    agent = {
        type = { "humanoid" }
    },
    target = {
        type = { "oneself" }
    }
}

function SGGS:Listener(data)
	if data.target ~= nil and data.shooter.dict.growShooter then
		data.target.Grow(0.005, 3)
	end
end

function SGGS:Start()
	
	local microCount = 10
	local chaseTarget = false
	
	micros = SpawnMicros(self, microCount, self.agent, self.Callback)
	
	for key, micro in pairs(micros) do
		
    end
	
	self.agent.dict.GrowHitListener = Event.Register(self, EventCode.OnAIRaygunHit, self.Listener)
end

function SGGS:Callback(micro)
	micro.EquipRaygun()
	-- simple check to make sure gun was actually equipped
	-- this is to prevent issues with models who don't have hand bones
	if micro.shooting ~= nil then
		if chaseTarget then
			micro.Aim(self.agent)
			micro.StartFiring()
		else
			micro.Engage(self.agent)
		end
		micro.shooting.SetProjectileColor(0, 255, 0)
		micro.dict.growShooter = true
	end
end

----------------------------------------------------------


SGSS = RegisterBehavior("SpawnShrinkSqaud")
SGSS.data = {
    menuEntry = "Spawn/Shrink Squad",
    secondary = true,
    agent = {
        type = { "humanoid" }
    },
    target = {
        type = { "oneself" }
    }
}

function SGSS:Listener(data)
    if data.target ~= nil and data.shooter.dict.shrinkShooter then
		data.target.Grow(-0.005, 3)
	end
end

function SGSS:Start()
	
	local microCount = 10
	local chaseTarget = false
	
	micros = SpawnMicros(self, microCount, self.agent, self.Callback)	
	self.agent.dict.ShrinkHitListener = Event.Register(self, EventCode.OnAIRaygunHit, self.Listener)
end

function SGSS:Callback(micro)
	micro.EquipRaygun()	
	if micro.shooting ~= nil then
		if chaseTarget then
			micro.Aim(self.agent)
			micro.StartFiring()
		else
			micro.Engage(self.agent)
		end
		micro.shooting.SetProjectileColor(0, 255, 0)
		micro.dict.shrinkShooter = true
	end
end

----------------------------------------------------------


SGTS = RegisterBehavior("SpawnGTSTakedownSqaud")
SGTS.data = {
    menuEntry = "Spawn/GTS Takedown Squad",
    secondary = true,
	flags = { "shooterDemo" },
    agent = {
        type = { "giantess" }
    },
    target = {
        type = { "oneself" }
    }
}

function SGTS:Listener(data)
    if data.target ~= nil and data.shooter.dict.takedownShooter then
		if self.hp > 0 then
			self.hp = self.hp - 5
		end
	end
end

function SGTS:Start()
	self.hp = 1000
	self.state = "okay"
	
	local microCount = 10
	
	self.micros = SpawnMicros(self, microCount, self.agent, self.Callback)
	self.agent.dict.TakedownHitListener = Event.Register(self, EventCode.OnAIRaygunHit, self.Listener)

end

function SGTS:Callback(micro)
	micro.EquipRaygun()
	if micro.shooting ~= nil then
		micro.Engage(self.agent)
		micro.shooting.SetProjectileColor(255, 0, 0)
		micro.dict.takedownShooter = true
	end
end

function SGTS:LazyUpdate()
	if self.state == "okay" then
		if self.hp < 1 then
			self.state = "kneeling"
			self.agent.animation.SetAndWait("Standing to Kneel Collapse")
			self.agent.animation.Set("Kneeling Idle")
		end
	elseif self.state == "kneeling" then
		self.agent.Wait(10)
		if math.random(3) > 2 then --recover
			self.state = "okay"
			self.hp = 300
			self.agent.animation.SetAndWait("Kneeling Recover")
			self.agent.animation.Set("Walk")
			self.agent.Wander(10)
		else --collapse and flag exit
			self.state = "dead"
			self.agent.animation.SetAndWait("Kneeling Collapse")
		end
	elseif self.state == "dead" then -- terminate script
		if not self.agent.ai.HasQueuedActions() then
			self.agent.animation.Set("Collapsed Idle")
			self.agent.ai.StopSecondaryBehavior("shooterDemo")
		end
	end
end



function SGTS:Exit()
	for key, micro in pairs(self.micros) do
		micro.UnequipGun()
		micro.animation.Set("Victory")
	end
end