package com.example.walkability.api.models

import com.google.gson.annotations.SerializedName

data class IndicatorsResponse(
    @SerializedName("indicators")
    val indicators: List<IndicatorResponse>,
    @SerializedName("description")
    val description: String
)

data class IndicatorResponse(
    @SerializedName("name")
    val name: String,
    @SerializedName("description")
    val description: String,
    @SerializedName("group")
    val group: String,
    @SerializedName("datasetName")
    val datasetName: String,
    @SerializedName("parameterLabel")
    val parameterLabel: String?,
    @SerializedName("defaultParameter")
    val defaultParameter: String?
)