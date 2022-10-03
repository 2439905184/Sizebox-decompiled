local growthModes = {}

growthModes.CONSTANT = {
    multiplier = 0.05   -- grow by a constant percentage, not affected by victim's size
}

growthModes.LINEAR = {
    multiplier = 2.5,   -- gain victim's HEIGHT multiplied by this factor
    power = 1           -- this mode features moderate growth at small victim/gts scale
}                       -- differences, that decreases only slightly as scale difference rises

growthModes.QUADRIC = {
    multiplier = 256,   -- gain victim's AREA multiplied by this factor
    power = 2           -- this mode features fast growth at small victim/gts scale differences
}                       -- and slow growth at larger scale differences

growthModes.CUBIC = {
    multiplier = 19700, -- gain victim's VOLUME multiplied by this factor
    power = 3           -- this mode features dramatic growth at small victim/gts scale differences
}                       -- and very slow growth at larger scale differences


function growthModes.getFactor(mode, victimScale, crusherScale)
    if mode == CONSTANT then
        return mode.multiplier
    else
        local scaleRatio = victimScale / crusherScale
        return math.pow(1 + math.pow(scaleRatio, mode.power) * mode.multiplier, 1/mode.power) - 1
    end
end


return growthModes