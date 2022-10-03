Grow = RegisterBehavior("Grow")
Grow.data = {
    menuEntry = "Size/Grow",
    secondary = true,
    agent = {
        type = { "humanoid" }
    },
    target = {
        type = { "oneself" }
    }
}

function Grow:Start() 
    self.agent.grow(0.1)
end