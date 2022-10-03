
MICRO_LIMIT = 50
PERIOD = 2
MICROS_PER_CYCLE = 5

microCount = 0
micros = {}
models = Entity.GetFemaleMicroList()

function Update()
    local spawner = globals["microSpawner"]

    if not spawner or not spawner.active or spawner.time + PERIOD > Time.time then return end

    spawner.time = spawner.time + PERIOD
    local origin = spawner.target and spawner.target.position or spawner.cursorPoint

    for i=1,math.min(MICROS_PER_CYCLE, MICRO_LIMIT - microCount) do
        local model = models[math.random(#models)] 
        local c = Random.insideUnitCircle
        local offset = Vector3.new(c.x, 0, c.y)
        local pos = origin + offset * spawner.radius
        local rot = Quaternion.angleAxis(math.random(360), Vector3.up)
        local e = Entity.SpawnFemaleMicro(model, pos, rot, spawner.scale)
        micros[e] = true
        microCount = microCount + 1
    end

    for e,_ in pairs(micros) do
        if e == nil or e.IsCrushed() then
            micros[e] = nil
            microCount = microCount - 1
        end
    end
end


