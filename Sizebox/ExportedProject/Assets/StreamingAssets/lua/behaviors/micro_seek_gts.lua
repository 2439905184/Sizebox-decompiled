MicroSeek = RegisterBehavior("Micro Seek")
MicroSeek.scores = {
    curious = 100,   --[[ the higher the value the more likely to choose that action ]]
    afraid = -150,
    hostile = 30,
}
MicroSeek.data = {
    hideMenu = true,
    agent = {
        type = { "micro" }
    },
    target = {
        type = { "humanoid" }
    }
}

minTime = 5 -- min time to wait (seconds)
maxTime = 10  -- max time to wait (seconds)
animation = "Walk" -- name of the walk animation
idleAnimation = "Idle"

function MicroSeek:Start()
    self.agent.animation.Set(animation) -- set the walk animation
    local time = math.random(minTime, maxTime) -- choose a random number between minTime and maxTime
    self.agent.seek(self.target, time) -- seek the giantess for the selected amount of time
end

function MicroSeek:Exit()
    self.agent.animation.Set(idleAnimation)
end