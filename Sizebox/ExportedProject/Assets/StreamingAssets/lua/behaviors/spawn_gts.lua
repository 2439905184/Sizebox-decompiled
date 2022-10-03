SpawnGTS = RegisterBehavior("Spawn Giantesses")
SpawnGTS.data = {
    menuEntry = "Spawn/Giantesses",
    agent = {
        type = { "player" }
    },
    target = {
        type = { "oneself" }
    }
}

giantessCount = 6    -- how many giantesses to spawn
radius = 25          -- radius of a circle around player where giantess will spawn
scale = 50           -- spawned giantess scale

function SpawnGTS:Start()
    models = Entity.GetGtsModelList()                           -- giantess model list
    model = models[math.random(#models)]                        -- pick random model from list
    pos = self.agent.position                                   -- our position
    angle = self.agent.transform.rotation.eulerAngles.y         -- our facing
    angle = angle + 180                                         -- turn around

    for i=1,giantessCount do
        -- model = models[math.random(#models)]       -- uncomment to choose a different model for every giantess
        gtsAngle = angle + (i-1) * 360 / giantessCount                      -- angle for this giantess
        gtsRot = Quaternion.angleAxis(gtsAngle, Vector3.up)                 -- rotation quaternion
        gtsPos = pos - gtsRot * Vector3.forward * radius                    -- rotate a vector of radius length and add to player position
        gts = Entity.spawnGiantess(model, gtsPos, gtsRot, scale)            -- spawn a giantess
        gts.lookAt(self.agent)
        gts.wait(1)
        gts.stomp(self.agent)
        -- gts.ai.setBehavior("Stomp", self.agent)
    end
end

