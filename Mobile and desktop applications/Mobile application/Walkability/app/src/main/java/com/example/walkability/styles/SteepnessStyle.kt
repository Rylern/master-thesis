package com.example.walkability.styles

import com.mapbox.maps.Style
import com.mapbox.maps.extension.style.expressions.dsl.generated.*
import com.mapbox.maps.extension.style.layers.addLayer
import com.mapbox.maps.extension.style.layers.generated.lineLayer
import com.mapbox.maps.extension.style.layers.properties.generated.LineCap
import com.mapbox.maps.extension.style.layers.properties.generated.LineJoin
import com.mapbox.maps.extension.style.sources.addSource
import com.mapbox.maps.extension.style.sources.generated.geoJsonSource

object SteepnessStyle {
    fun setStyle(style: Style, sourceID: String, layerID: String, max: Float) {
        val minSteepness = 4.0
        var maxSteepness = if (max < minSteepness) minSteepness else max.toDouble()
        maxSteepness += 0.01
        val midSteepness = (maxSteepness - minSteepness) / 2.0 + minSteepness

        style.addSource(geoJsonSource(sourceID) {})
        style.addLayer(lineLayer(layerID, sourceID) {
            lineCap(LineCap.ROUND)
            lineJoin(LineJoin.ROUND)
            lineOpacity(0.7)
            lineWidth(
                interpolate {
                    linear()
                    zoom()
                    stop {
                        literal(0)
                        literal(0)
                    }
                    stop {
                        literal(22)
                        literal(5)
                    }
                }
            )
            lineColor(
                rgb {
                    interpolate {
                        linear()
                        get("steepness")
                        stop {
                            literal(minSteepness)
                            literal(145)
                        }
                        stop {
                            literal(midSteepness)
                            literal(255)
                        }
                        stop {
                            literal(maxSteepness)
                            literal(255)
                        }
                        stop {
                            literal(maxSteepness+0.01)
                            literal(255)
                        }
                    }
                    interpolate {
                        linear()
                        get("steepness")
                        stop {
                            literal(minSteepness)
                            literal(255)
                        }
                        stop {
                            literal(midSteepness)
                            literal(255)
                        }
                        stop {
                            literal(maxSteepness)
                            literal(119)
                        }
                        stop {
                            literal(maxSteepness+0.01)
                            literal(0)
                        }
                    }
                    interpolate {
                        linear()
                        get("steepness")
                        stop {
                            literal(minSteepness)
                            literal(114)
                        }
                        stop {
                            literal(midSteepness)
                            literal(118)
                        }
                        stop {
                            literal(maxSteepness)
                            literal(77)
                        }
                        stop {
                            literal(maxSteepness+0.01)
                            literal(0)
                        }
                    }
                }
            )
        })
    }
}