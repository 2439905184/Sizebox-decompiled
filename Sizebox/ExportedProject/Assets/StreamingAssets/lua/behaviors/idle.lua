GiantessIdle = RegisterBehavior("Idle")
GiantessIdle.scores = {
    normal = 30
}
GiantessIdle.data = {
    agent = {
        type = { "humanoid" }
    },
    target = {
        type = { "oneself" }
    },
	tags = "macro, micro, animation",
}

--[[ you can define global data outside of the function
 this data will be shared by all characters
 if you change something in runtime all characters will be
 affected by it ]] --

 --[[ to declare local variables use the name of the GiantessIdle.variableName = "data", or create the
 self.variableName = "data" inside a function ]]

 --[[ all entities will use the same set of animation, so is ok to declare this
 outside of the function ]]--

gtsAnimList = {"Crossarms", "Embar", "Embar 2", "Greet 2", "Greet 3", "Greet 4", "Victory", "Wait Gesture",
    "Jump Low", "Look Down", "Look Around 2", "Pick Up", "Refuse", "Scratch Head", 
    "Thinking", "Wait Strech Arms", "Wait Torso Twist"}

microAnimList = {"Crossarms", "Embar", "Embar 2", "Greet 3", "Greet 4", "Idle 2", 
    "Jump Low", "Look Down", "Pick Up", "Refuse", "Scratch Head",
    "Thinking", "Wait Strech Arms", "Wait Torso Twist"}

function GiantessIdle:Start()
    if self.agent.isGiantess() then
        animationList = gtsAnimList
    else
        animationList = microAnimList
    end
    local index = math.random(#animationList) -- gets a random index from the animation list
    local animation = animationList[index] -- gets an animation form the list with the previous index
    self.agent.animation.SetAndWait(animation) -- plays the animation until it ends
end