package com.example.walkability.api.models

import com.google.gson.annotations.SerializedName

data class IndicatorRequest(
    @SerializedName("datasetName")
    val datasetName: String,
    @SerializedName("weight")
    val weight: Float,
    @SerializedName("parameter")
    val parameter: Float?
)