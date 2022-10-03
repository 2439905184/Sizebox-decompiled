Rampage = RegisterBehavior("Rampage")
Rampage.scores = {
    hostile = 100,     --[[ the higher the value the more likely to choose that action ]]
    curious = -30,
}
Rampage.data = {
    agent = {
        type = { "giantess" }
    },
    target = {
        type = { "oneself" }
    }
}
    
IDLE_ANIMATION = "Idle 2"

function Rampage:Start()
    self.stop = false -- i added a stop variable to end the behavior.. this is custom for this script
end

function Rampage:Update()
    if not self.agent.ai.IsActionActive() then

        if self.stop then
            self.agent.ai.StopBehavior() -- if you use Update() you must manually tell when to end the behavior, after this the Exit() method will run
            return
        else

        self.target = self.agent.FindRandomBuilding(self.agent) --  check if there is any building nearby
        log("got target data")
        
        if not self.target then
            self.stop = true    --  there are no more buildings near the gts, stop the script
            log("No target was found, Rampage end")
            return
        end

        log("target found, setting out to destroy")
        self.agent.animation.Set("Walk")        
        self.agent.MoveTo(self.target)   --  move to the target, buildings are static so use the cheapest call possible
        self.agent.animation.Set(IDLE_ANIMATION)
        self.agent.Wreck()          --  here you have reached the structure, now destroy it

        -- if the giantess has the AI mode active or another behavior queued, 
        -- then i want her to stop and do another thing
        -- it will stop in the next loop, so i can make sure it runs at least once
        self.stop = self.agent.ai.IsAIEnabled() or self.agent.ai.HasQueuedBehaviors() 
        end
    end
end

function Rampage:Exit()
    self.agent.animation.Set(IDLE_ANIMATION)
end
