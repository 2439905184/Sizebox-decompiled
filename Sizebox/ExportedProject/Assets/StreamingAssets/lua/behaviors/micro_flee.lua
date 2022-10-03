MicroFlee = RegisterBehavior("Flee")
MicroFlee.react = true -- this actions is high priority, so they will react interrupting the previous action
-- probably it will be changed for a priority system
MicroFlee.scores = {
    afraid = 100, --[[ the higher the value the more likely to choose that action ]]
}

MicroFlee.data = {
    agent = {
        type = { "micro" }, 
    },
    target = {
        type = { "humanoid" }
    }
}

minTime = 4 -- min time to wait (seconds)
maxTime = 8  -- max time to wait (seconds)
animation = "Run" -- name of the walk animation
tiredAnimation = "Reflesh" -- when is tired of run
idleAnimation = "Idle"

function MicroFlee:Start()
    self.agent.animation.Set(animation) -- set the walk animation
    local time = math.random(minTime, maxTime) -- choose a random number between minTime and maxTime
    self.agent.flee(self.target, time) -- flee from the target for "time" seconds
    self.agent.animation.SetAndWait(tiredAnimation)
end

function MicroFlee:Exit()
    self.agent.animation.Set(idleAnimation)
end
