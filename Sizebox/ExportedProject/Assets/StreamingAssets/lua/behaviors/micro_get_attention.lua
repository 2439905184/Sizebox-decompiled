MicroGetAttention = RegisterBehavior("Micro Get Attention")
MicroGetAttention.scores = {
    curious = 100,   --[[ the higher the value the more likely to choose that action ]]
    afraid = 30,
}
MicroGetAttention.data = {
    hideMenu = true,
    forceAppearInManager = true,
    agent = {
        type = { "micro" } 
    },
    target = {
        type = { "humanoid" }
    }
}

--[[ you can define global data outside of the function
this data will be shared by all characters
if you change something in runtime all characters will be
affected by it ]] --

--[[ all micros will use the same set of animation, so is ok to declare this
outside of the function ]]--

animationList = {
    "Greet 3", 
    "Greet 4", 
    "Hand Raising",
    "Jump 2",
    "Jump Low",
    "Try To Catch Up",
    "Victory Idle",
    "Waving 2",
}

walkAnimation = "Walk"

function MicroGetAttention:Start()
    local index = math.random(#animationList) -- gets a random index from the animation list
    local animation = animationList[index] -- gets an animation form the list with the previous index
    self.agent.animation.Set(walkAnimation)
    self.agent.LookAt(self.target)
    self.agent.Face(self.target)
    self.agent.seek(self.target, 10, 12)
    self.agent.animation.SetAndWait(animation) -- plays the animation until it ends
    self.agent.animation.SetAndWait(animation)
    self.agent.animation.SetAndWait(animation)
end
