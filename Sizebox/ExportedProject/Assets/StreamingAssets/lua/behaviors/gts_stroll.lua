Stroll = RegisterBehavior("Stroll")
Stroll.scores = {
    hostile = 20,     --[[ the higher the value the more likely to choose that action ]]
    curious = -30,
}
Stroll.data = {
	menuEntry = "Walk/Stroll",
    agent = {
        type = { "giantess" }
    },
    target = {
        type = { "oneself" }
    },
	tags = "macro, movement, evil",
    settings = {
        {"limitDistance", "Limit Search Radius", "float", 0}
    }
}
    
IDLE = "Neutral Idle"
WALK = "Walk"
TAUNTS = {"Air Squat","Bashful","Bored","Crazy Gesture","Dismissing Gesture","Dismissing Gesture 2","Excited","Fist Pump","Greet","Happy","Insult","Jump 3","Loser","Refuse","Salute 2","Taunt 3","Thankful","Whatever Gesture"}

function Stroll:Start()
    self.stop = false --A stop variable to end the behavior... This is custom for this script
    if self.limitDistance == 0 then --If search radius is disabled
        self.target = self.agent.FindRandomBuilding(self.agent) --Check if theres a building to walk into
        if not self.target then
		    self.stop = true --There are no buildings near the gts, stop the script
		    return --Return early, don't bother making a seed since we're not going to run anyway
	    end
    else
        self.target = nil --If search radius enabled, prepare for next part
    end
    self.searchTimer = Time.time
    self.searchCounter = 0
    self.searchCounter2 = 0
    --self.lookingAtPasserby = false
    --log(self.limitDistance)
	Random.InitState(Mathf.Round(Time.timeSinceLevelLoad)*self.agent.id) -- get a somewhat random seed for the animations
end

function Stroll:Update()
    --[[if self.passerby then
        if self.lookingAtPasserby then --If looking at passerby
            if self.agent.DistanceTo(self.passerby) > self.agent.scale * 2.2 then --If passerby too far or in the back
                self.lookingAtPasserby = false
                log("passerby too far back")
            end
        else
            self.passerby = self.agent.FindClosestMicro() --Choose a new passerby
            log("looking for a good boi")
            if self.agent.DistanceTo(self.passerby) < self.agent.scale * 2 then --If passerby close and can be seen, choose him
                self.agent.lookAt(self.passerby)
                self.lookingAtPasserby = true
                log("found new passerby")
            else
                self.agent.lookAt(nil)
                log("none suitable")
            end
        end
    end--]]

    if not self.agent.ai.IsActionActive() then 
        if self.stop then
            log("No building was found for the stroll script")
            self.agent.ai.StopBehavior() -- if you use Update() you must manually tell when to end the behavior, after this the Exit() method will run
            return
        else
            if self.limitDistance == 0 then --If search radius is disabled, just pick any building
                self.target = self.agent.FindRandomBuilding(self.agent)
            else
                if self.searchCounter >= 50 then --After enough attempts, let the search take a break.
                    
                    self.searchCounter = 0
                    self.searchCounter2 = self.searchCounter2 + 1

                    if self.searchCounter2 >= 20 then
                        log("No buildings found after 1000 attempts.")
                        log("Consider raising your search radius, or growing a little...")                
                        --log("Attempting again in 5 seconds...")
                        self.searchCounter2 = 0
                        self.searchTimer = Time.time + 5
                    elseif self.agent.ai.IsAIEnabled() and self.searchCounter2 >= 3 then
                        self.stop = true
                        --log("AI Enabled, only looping 3 times.")
                        return
                    else
                        --log("No buildings found after 50 attempts.")
                        --log("Attempting again in 2 seconds...")
                        self.agent.animation.Set(IDLE)
                        self.searchTimer = Time.time + 2
                        return
                    end

                elseif Time.time >= self.searchTimer then --Attempt to find closer building
                    
                    self.target = self.agent.FindRandomBuilding(self.agent)

                    if not self.target then
                        self.stop = true
                        return
                    end
                    
                    self.minBuildingDistance = self.agent.scale * self.limitDistance * 100
                    
                    if self.agent.DistanceTo(self.target) > self.minBuildingDistance then
                        self.target = nil
                        self.searchCounter = self.searchCounter + 1
                        --log(self.searchCounter)
                        return
                    end            
                end
            end
            if self.target then
                --[[if not self.passerby then 
                    self.passerby = self.agent.FindClosestMicro()
                    if self.agent.DistanceTo(self.passerby) < self.agent.scale * 4 then
                        self.agent.lookAt(self.passerby)
                        self.lookingAtPasserby = true
                        log("found the one!")
                    else
                        self.agent.lookAt(nil)
                        log("no micros nearby first pass")
                    end
                end--]]

                self.agent.lookAt(self.agent.FindClosestMicro())
                self.agent.animation.Set(WALK)
                self.agent.MoveTo(self.target)   -- Move to the target, buildings are static so use the cheapest call possible
                self.agent.animation.SetAndWait(TAUNTS[Mathf.Round(Random.Range(1,#TAUNTS))]) -- Play a random animation from the array
                self.agent.animation.Set(IDLE)
                self.searchCounter = 0
                self.searchCounter2 = 0
                -- if the giantess has the AI mode active or another behavior queued, 
                -- then i want her to stop and do another thing
                -- it will stop in the next loop, so i can make sure it runs at least once
                self.stop = self.agent.ai.IsAIEnabled() or self.agent.ai.HasQueuedBehaviors() 
            else
                if self.limitDistance == 0 then
                    self.stop = true
                return
                end
            end
        end
    end
end

function Stroll:Exit()
    --log("quit stroll")
    if not self.agent.ai.IsAIEnabled() then -- Don't set an animation if AI is enabled. This causes unnecessary twitching
        self.agent.animation.Set(IDLE)      -- because the GTS transitions into an animation for only a few frames
    end
end
