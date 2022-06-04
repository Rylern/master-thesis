package com.example.walkability.api.models

import com.google.gson.annotations.SerializedName


data class HeatmapResponse(
    @SerializedName("features")
    val features : List<HeatmapFeature>,
    @SerializedName("type")
    val type: String
)

data class HeatmapFeature(
    @SerializedName("bbox")
    val bbox : List<Double>,
    @SerializedName("geometry")
    val geometry: Geometry,
    @SerializedName("id")
    val id: Int,
    @SerializedName("properties")
    val properties: HeatmapProperty,
    @SerializedName("type")
    val type: String
)

data class HeatmapProperty(
    @SerializedName("indicator")
    val indicator : Double
)