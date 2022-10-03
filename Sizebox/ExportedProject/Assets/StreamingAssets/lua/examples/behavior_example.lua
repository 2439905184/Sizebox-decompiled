
-- Register your behavior. It will act as a class, so every agent doing the action will have an instance of this action
-- The instance will be destroyed when the action ends, if you need to store data use a global variables, but keep in
-- mind that will be shared by all the instances

MyBehavior = RegisterBehavior("My Behavior")
MyBehavior.agentType = "giantess" -- wich kind of agent will do the action
MyBehavior.targetType = "micro" -- the target of the action ("none": would not ask a target, "oneself": the agent is the target)
MyBehavior.scores = { 
    hostile = 20,  -- the scores to choose if the action will be done.. the score system is most likely to be changed in the future
    curiosity = 20, -- but you have to set up some values at least if you want to test the autonomous AI
    fear = 20, -- the scores must be between 0 and 100
    normal = 20
}

-- This is the first method, at the start of the behavior execution, it will run only one time
-- You initialize all the variables that you need, use the self to access your
function MyBehavior:Start()
    self.startTime = time.time 
    log(self.agent.name .. " start this behavior")
end

-- This will be executed at every frame, use it if you actions is more complex and needs to wait for an event to occur, take
-- desitions, or manually update the position every frame. 
function MyBehavior:Update()
    local timeFromStart = time.time - self.startTime
    log("Time since start of behavior: " .. timeFromStart)
    if timeFromStart > 2 then
        log("more than 2 seconds have passed")
        self.agent.ai.StopBehavior() -- if you use the update function, you need to explicitely tell when to end the behavior, that's because you may want want it to not stop
    end                               -- or run from a determined amount of time
end
-- for now, if there is not a current action the behavior will end automatically (if is being used by the ai)
-- i need to add a function to explicitely tell when the behavior must end

-- This will be execute by the following reasons: the behaviors ends, the behavior is canceled by the player by choosing another
-- behavior, or the behavior is interrupted by another behavior of higher priority, you can revert the changes, or make the character
-- play and idle animation
function MyBehavior:Exit()
    log("goodbye")
end




-- you can define multiple behavior in the same script
-- you don't need to include all methods, for example if you don't need Update()
-- if you are porting previous scripts.. Start() is the equivalent of Main(), as it only runs one time
TestBehavior = RegisterBehavior("Test")
TestBehavior.agentType = "giantess"
TestBehavior.targetType = "micro"

function TestBehavior:Start() 
    log("hello world")
end