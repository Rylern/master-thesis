package com.example.walkability.models

import java.io.Serializable

class Indicator (
    val name: String,
    val description: String = "",
    val group: String = "",
    val datasetName: String = "",
    var weight: Float = 100.0f,
    val parameterLabel: String = "",
    val defaultParameterValue: String = "",
    var parameterValue: Float = -1f): Serializable