Grab = RegisterBehavior("Grab")
-- scores disabled until grab is fixed
-- Grab.scores = {
--     normal = 10,    --[[ the scores are set this way, for each personality or state.. is a value from 0 to 100 ]]
--     curiosity = 100 --[[ the higher the value the more likely to choose that action ]]
-- }
Grab.data = {
    agent = {
        type = { "giantess" }
    },
    target = {
        type = { "micro" }
    }
}

function Grab:Start()
    self.stop = false
    self.defaultLowerLegPos = self.agent.bones.leftLowerLeg.position.y - self.agent.transform.position.y
	
	if self.agent.height < self.target.height * 5 then
		self.agent.animation.SetAndWait("No 2",true)
		print("Target is too large to grab!")
		self.agent.animation.Set("Idle 2",true)
		self.stop = true
	end
end

function Grab:Update()
    if not self.agent.ai.IsActionActive() then
        if self.stop then
            self.agent.ai.StopBehavior() -- if you use Update() you must manually tell when to end the behavior, after this the Exit() method will run
            return
        else
            if not self.target or not self.target.IsTargettable() then -- when looping the action, it needs to change the self.target when
                self.target = self.agent.findClosestMicro()    -- the first self.target is dead
                if not self.target then
                    self.agent.ai.StopBehavior() -- if it can't find a new self.target, then cancel the action
                    return
                end
            end
			
			local crouch = false
			local closest = self.agent.scale * 0.4
			if self.target.transform.position.y <= self.agent.transform.position.y + self.defaultLowerLegPos then
				crouch = true
				closest = self.agent.scale * 0.5
			end

			if self.agent.DistanceTo(Vector3.New(self.target.position.x, self.agent.position.y, self.target.position.z)) > closest then
				if not self.chasing then
					self.chasing = true
					self.agent.ai.StopAction()
					self.agent.LookAt(self.target)
					self.agent.animation.Set("Walk",true)
					self.agent.MoveTo(self.target)
					self.agent.Face(self.target)
					
					-- This ones being troublesome to catch; Try looking for another target.
					self.stop = true
				end
			else
				if self.chasing or self.crouching != crouch then
					self.chasing = false
					self.crouching = crouch
					
					-- Figure out whether we should crouch or not.
					if crouch then
						self.agent.animation.Set("Crouch Idle",true)
					else
						self.agent.ai.StopAction()
						self.agent.animation.Set("Idle 4",true)
					end
					
					self.agent.grab(self.target)
				end
			end

            -- if the giantess has the AI mode active or another behavior queued, 
            -- then i want her to stop and do another thing
            -- it will stop in the next loop, so i can make sure it runs at least once
            self.stop = self.agent.ai.IsAIEnabled() or self.agent.ai.HasQueuedBehaviors() 
        end
    elseif not self.chasing and self.target.transform.IsChildOf(self.agent.transform) then
		if self.agent.animation.Get() == "Crouch Idle" then
			self.agent.ai.StopAction()
			self.agent.animation.Set("Idle 4",true)
		end
	end
end
