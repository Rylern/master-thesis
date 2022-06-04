package com.example.walkability.styles

import com.mapbox.maps.Style
import com.mapbox.maps.extension.style.expressions.dsl.generated.interpolate
import com.mapbox.maps.extension.style.expressions.dsl.generated.rgb
import com.mapbox.maps.extension.style.layers.addLayer
import com.mapbox.maps.extension.style.layers.generated.lineLayer
import com.mapbox.maps.extension.style.layers.properties.generated.LineCap
import com.mapbox.maps.extension.style.layers.properties.generated.LineJoin
import com.mapbox.maps.extension.style.sources.addSource
import com.mapbox.maps.extension.style.sources.generated.geoJsonSource

object HeatmapStyle {
    fun setStyle(style: Style, sourceID: String, layerID: String) {
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
                        get("indicator")
                        stop {
                            literal(0)
                            literal(255)
                        }
                        stop {
                            literal(0.5)
                            literal(255)
                        }
                        stop {
                            literal(1)
                            literal(145)
                        }
                    }
                    interpolate {
                        linear()
                        get("indicator")
                        stop {
                            literal(0)
                            literal(119)
                        }
                        stop {
                            literal(0.5)
                            literal(255)
                        }
                        stop {
                            literal(1)
                            literal(255)
                        }
                    }
                    interpolate {
                        linear()
                        get("indicator")
                        stop {
                            literal(0)
                            literal(77)
                        }
                        stop {
                            literal(0.5)
                            literal(118)
                        }
                        stop {
                            literal(1)
                            literal(114)
                        }
                    }
                }
            )
        })
    }
}