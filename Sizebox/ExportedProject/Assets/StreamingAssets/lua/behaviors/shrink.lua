Shrink = RegisterBehavior("Shrink")
Shrink.data = {
    menuEntry = "Size/Shrink",
    secondary = true,
    agent = {
        type = { "humanoid" }
    },
    target = {
        type = { "oneself" }
    }
}

function Shrink:Start() 
    self.agent.grow(-0.1)
end