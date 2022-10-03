
radius = 25          -- radius of a circle around player where giantess will spawn
scale = 50           -- spawned giantess scale

function Start()
    models = Entity.GetGtsModelList()                           -- giantess model list
end

function Update()
    if Input.GetKeyDown("g") then
        SpawnGts();
    end
end

function SpawnGts()
    local model = models[math.random(#models)]              -- pick random model from list 
    Entity.spawnGiantess(model, scale)      -- spawn a giantess
end