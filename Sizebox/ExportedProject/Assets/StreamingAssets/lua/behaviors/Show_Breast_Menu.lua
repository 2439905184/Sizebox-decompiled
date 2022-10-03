BreastMenu = RegisterBehavior("Breast_Physics_Update")
BreastMenu.data = {
    menuEntry = "Debug/Breast Physics Options",
    secondary = true,
    flags = { "debug" },
    agent = {
        type = { "giantess" }
    },
    target = {
        type = { "oneself" }
    }
}

function BreastMenu:Start()
    Log("Showing breast physics menu") 
    self.agent.ShowBreastPhysicsOptions()
end
