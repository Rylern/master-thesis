package com.example.walkability.api.models

import com.google.gson.annotations.SerializedName


data class SteepnessResponse(
    @SerializedName("features")
    val features : List<SteepnessFeature>,
    @SerializedName("type")
    val type: String
)

data class SteepnessFeature(
    @SerializedName("bbox")
    val bbox : List<Double>,
    @SerializedName("geometry")
    val geometry: Geometry,
    @SerializedName("id")
    val id: Int,
    @SerializedName("properties")
    val properties: SteepnessProperty,
    @SerializedName("type")
    val type: String
)

data class SteepnessProperty(
    @SerializedName("fid")
    val fid : Int,
    @SerializedName("steepness")
    val steepness : Double
)