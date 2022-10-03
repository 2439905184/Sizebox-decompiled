CollisionUpdate = RegisterBehavior("force_collider_update")
CollisionUpdate.data = {
    menuEntry = "Debug/Update Colliders",
    secondary = true,
    flags = { "debug" },
    agent = {
        type = { "giantess" }
    },
    target = {
        type = { "oneself" }
    }
}

function CollisionUpdate:Start()
    Log("Updating mesh collider...")
    self.agent.UpdateMeshCollider()
end
