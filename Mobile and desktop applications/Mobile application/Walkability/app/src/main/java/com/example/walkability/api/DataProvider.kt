package com.example.walkability.api

import com.example.walkability.api.models.IndicatorRequest
import com.example.walkability.models.Indicator
import com.mapbox.geojson.Point
import okhttp3.OkHttpClient
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import java.util.concurrent.TimeUnit

object DataProvider {

    private const val baseURL = "http://51.222.155.135:10000/"
    //private const val baseURL = "http://192.168.0.107:8000/"

    private val okHttpClient = OkHttpClient.Builder()
        .connectTimeout(30, TimeUnit.SECONDS)
        .readTimeout(30, TimeUnit.SECONDS)
        .writeTimeout(30, TimeUnit.SECONDS)
        .build()
    private var service = Retrofit.Builder()
        .baseUrl(baseURL)
        .client(okHttpClient)
        .addConverterFactory(GsonConverterFactory.create())
        .build()
        .create(ServiceAPI::class.java)

    suspend fun getHeatmap(indicators: List<Indicator>) = service.getHeatmap(indicators.map {
        if (it.parameterValue >= 0) {
            IndicatorRequest(it.datasetName, it.weight, it.parameterValue)
        } else {
            IndicatorRequest(it.datasetName, it.weight, null)
        }
    })

    suspend fun getNavigation(indicators: List<Indicator>, to: Point, from: Point) = service.getNavigation(indicators.map {
        if (it.parameterValue >= 0) {
            IndicatorRequest(it.datasetName, it.weight, it.parameterValue)
        } else {
            IndicatorRequest(it.datasetName, it.weight, null)
        }
    }, to.latitude(), to.longitude(), from.latitude(), from.longitude())

    suspend fun getIndicators(language: String, advancedMode: Boolean) = service.getIndicators(language, advancedMode)

    suspend fun getSteepness() = service.getSteepness()

    suspend fun saveHeatmap(indicators: List<Indicator>) = service.saveHeatmap(indicators.map {
        if (it.parameterValue >= 0) {
            IndicatorRequest(it.datasetName, it.weight, it.parameterValue)
        } else {
            IndicatorRequest(it.datasetName, it.weight, null)
        }
    })
}