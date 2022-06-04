package com.example.walkability.api.models

import com.google.gson.annotations.SerializedName

data class NavigationResponse(
    @SerializedName("features")
    val features : List<NavigationFeature>,
    @SerializedName("type")
    val type: String
)

data class NavigationFeature(
    @SerializedName("bbox")
    val bbox : List<Double>,
    @SerializedName("geometry")
    val geometry: Geometry,
    @SerializedName("id")
    val id: Int,
    @SerializedName("properties")
    val properties: NavigationProperty,
    @SerializedName("type")
    val type: String
)

data class Geometry(
    @SerializedName("coordinates")
    val coordinates : List<List<Any>>,
    @SerializedName("type")
    val type: String
)

data class NavigationProperty(
    @SerializedName("cost")
    val cost : Double,
    @SerializedName("end")
    val end: String,
    @SerializedName("start")
    val start: String
)