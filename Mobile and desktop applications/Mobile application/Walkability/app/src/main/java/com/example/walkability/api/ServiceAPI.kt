package com.example.walkability.api

import com.example.walkability.api.models.*
import retrofit2.http.*

interface ServiceAPI {
    @POST("heatmap/")
    suspend fun getHeatmap(@Body indicators: List<IndicatorRequest>): HeatmapResponse

    @POST("navigation/")
    suspend fun getNavigation(
        @Body indicators: List<IndicatorRequest>,
        @Query("fromLatitude") fromLatitude: Double,
        @Query("fromLongitude") fromLongitude: Double,
        @Query("toLatitude") toLatitude: Double,
        @Query("toLongitude") toLongitude: Double): NavigationResponse

    @GET("indicators/")
    suspend fun getIndicators(@Query("language") language: String, @Query("advancedMode") advancedMode: Boolean): IndicatorsResponse

    @GET("steepness/")
    suspend fun getSteepness(): SteepnessResponse

    @POST("heatmap/save")
    suspend fun saveHeatmap(@Body indicators: List<IndicatorRequest>)
}