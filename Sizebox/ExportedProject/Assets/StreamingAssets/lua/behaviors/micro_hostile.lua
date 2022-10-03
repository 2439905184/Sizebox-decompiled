MicroHostile = RegisterBehavior("Micro Hostile")
MicroHostile.scores = {
    hostile = 100,   --[[ the higher the value the more likely to choose that action ]]
}
MicroHostile.data = {
    hideMenu = true,
    forceAppearInManager = true,
    agent = {
        type = { "micro" } 
    },
    target = {
        type = { "humanoid" }
    }
}

animationList = {
    "Goalie Throw",
    "Insult",
    "Shake Fist",
    "Taunt Gesture",
    "Threatening",
}

walkAnimation = "Walk"

function MicroHostile:Start()
    local animation = animationList[math.random(#animationList)] -- gets a random animation form the list
    self.agent.animation.Set(walkAnimation)
    self.agent.LookAt(self.target)
    self.agent.Face(self.target)
    self.agent.seek(self.target, 10, 15)
    self.agent.animation.SetAndWait(animation) -- plays the animation until it ends
    self.agent.animation.SetAndWait(animation)
    self.agent.animation.SetAndWait(animation)
end
