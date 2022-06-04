package com.example.walkability.fragments

import android.content.Context
import android.os.Bundle
import android.util.Log
import android.view.*
import android.widget.TextView
import androidx.constraintlayout.widget.ConstraintLayout
import androidx.core.view.isVisible
import androidx.fragment.app.Fragment
import com.example.walkability.MainActivity
import com.example.walkability.R
import com.example.walkability.api.models.NavigationResponse
import com.example.walkability.styles.HeatmapStyle
import com.example.walkability.styles.SteepnessStyle
import com.google.gson.Gson
import com.mapbox.android.core.permissions.PermissionsListener
import com.mapbox.android.core.permissions.PermissionsManager
import com.mapbox.geojson.Point
import com.mapbox.maps.*
import com.mapbox.maps.extension.style.layers.addLayer
import com.mapbox.maps.extension.style.layers.generated.lineLayer
import com.mapbox.maps.extension.style.layers.properties.generated.LineCap
import com.mapbox.maps.extension.style.layers.properties.generated.LineJoin
import com.mapbox.maps.extension.style.sources.addSource
import com.mapbox.maps.extension.style.sources.generated.GeoJsonSource
import com.mapbox.maps.extension.style.sources.generated.geoJsonSource
import com.mapbox.maps.extension.style.sources.getSourceAs
import com.mapbox.maps.extension.style.style
import com.mapbox.maps.plugin.animation.MapAnimationOptions.Companion.mapAnimationOptions
import com.mapbox.maps.plugin.animation.easeTo
import com.mapbox.maps.plugin.locationcomponent.location
import java.io.*


class MapFragment : Fragment() {
    private lateinit var mapView: MapView
    private lateinit var permissionsManager: PermissionsManager
    private lateinit var stopNavigation: MenuItem
    private lateinit var legendTitle: TextView
    private lateinit var legendImage: View

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        return inflater.inflate(R.layout.fragment_map, container, false)
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        mapView = view.findViewById(R.id.mapView)
        stopNavigation = (requireActivity() as MainActivity).stopNavigation
        stopNavigation.isEnabled = false
        legendTitle = view.findViewById(R.id.legendTitle)
        legendImage = view.findViewById(R.id.legendImage)

        setUpMap()
        setUpLegend(view)
        askForLocation()
        initMapCamera()
        displayLocation()
        setUpSteepnessAndIndicatorsToggle()

        val directions = requireArguments().getString("directions")
        if (directions != null) {
            displayNavigation(directions)
        }
    }

    private fun setUpMap() {
        mapView.getMapboxMap().loadStyle(
            style(styleUri = Style.MAPBOX_STREETS) {}
        )
    }

    private fun setUpLegend(view: View) {
        val sharedPrefSettings = requireActivity().getSharedPreferences(getString(R.string.settingsFile), Context.MODE_PRIVATE)
        val legend = view.findViewById<ConstraintLayout>(R.id.legend)
        val toggleLegend = (requireActivity() as MainActivity).toggleLegend
        if (sharedPrefSettings.getBoolean(getString(R.string.legendDisplayed), false)) {
            toggleLegend.isChecked = true
        }
        toggleLegend.setOnCheckedChangeListener { _, isChecked ->
            legend.isVisible = isChecked
            with (sharedPrefSettings?.edit()) {
                this?.putBoolean(getString(R.string.legendDisplayed), isChecked)
                this?.apply()
            }
        }
        legend.isVisible = toggleLegend.isChecked
    }

    private fun askForLocation() {
        if (!PermissionsManager.areLocationPermissionsGranted(requireActivity())) {
            permissionsManager = PermissionsManager(object : PermissionsListener {
                override fun onExplanationNeeded(permissionsToExplain: List<String>) {}
                override fun onPermissionResult(granted: Boolean) {}
            })
            permissionsManager.requestLocationPermissions(requireActivity())
        }
    }
    fun permissionResult(requestCode: Int, permissions: Array<String>, grantResults: IntArray) {
        permissionsManager.onRequestPermissionsResult(requestCode, permissions, grantResults)
    }

    private fun initMapCamera() {
        mapView.getMapboxMap().setCamera(CameraOptions.Builder()
            .center(INITIAL_CENTER)
            .zoom(INITIAL_ZOOM)
            .bearing(0.0)
            .build())
    }

    private fun displayLocation() {
        mapView.location.updateSettings {
            enabled = true
        }
    }

    private fun setUpSteepnessAndIndicatorsToggle() {
        val sharedPref = requireActivity().getSharedPreferences(getString(R.string.steepnessFile), Context.MODE_PRIVATE)
        val toggleIndicators = (requireActivity() as MainActivity).toggleIndicators
        val toggleSteepness = (requireActivity() as MainActivity).toggleSteepness

        val displaySteepness = sharedPref.getBoolean(getString(R.string.steepnessDisplayed), false)
        toggleIndicators.isChecked = !displaySteepness
        toggleSteepness.isChecked = displaySteepness

        toggleIndicators.setOnCheckedChangeListener {view, isChecked ->
            toggleSteepness.isChecked = !isChecked

            if (view.isPressed) {
                with (sharedPref?.edit()) {
                    this?.putBoolean(getString(R.string.steepnessDisplayed), !isChecked)
                    this?.apply()
                }
                if (isChecked) {
                    displayHeatmap()
                    legendTitle.text = getString(R.string.app_name)
                    legendImage.setBackgroundResource(R.drawable.heatmap_gradient)
                } else {
                    displaySteepness()
                    legendTitle.text = getString(R.string.steepness)
                    legendImage.setBackgroundResource(R.drawable.steepness_gradient)
                }
            }
        }

        toggleSteepness.setOnCheckedChangeListener { view, isChecked ->
            toggleIndicators.isChecked = !isChecked

            if (view.isPressed) {
                with (sharedPref?.edit()) {
                    this?.putBoolean(getString(R.string.steepnessDisplayed), isChecked)
                    this?.apply()
                }
                if (isChecked) {
                    displaySteepness()
                    legendTitle.text = getString(R.string.steepness)
                    legendImage.setBackgroundResource(R.drawable.steepness_gradient)
                } else {
                    displayHeatmap()
                    legendTitle.text = getString(R.string.app_name)
                    legendImage.setBackgroundResource(R.drawable.heatmap_gradient)
                }
            }
        }
        if (displaySteepness) {
            displaySteepness()
            legendTitle.text = getString(R.string.steepness)
            legendImage.setBackgroundResource(R.drawable.steepness_gradient)
        } else {
            displayHeatmap()
            legendTitle.text = getString(R.string.app_name)
            legendImage.setBackgroundResource(R.drawable.heatmap_gradient)
        }
    }

    fun displayHeatmap() {
        hideHeatmap()
        hideSteepness()

        mapView.getMapboxMap().getStyle { style ->
            HeatmapStyle.setStyle(style, ID_HEATMAP_SOURCE, ID_HEATMAP_LAYER)
        }

        val heatmapFile = File(requireContext().filesDir, getString(R.string.heatmapFile))
        if (heatmapFile.exists()) {
            mapView.getMapboxMap().getStyle {
                val geoJsonSource = it.getSourceAs<GeoJsonSource>(ID_HEATMAP_SOURCE)
                if (geoJsonSource != null) {
                    geoJsonSource.data(heatmapFile.readText(Charsets.UTF_8))
                    it.moveStyleLayer(ID_HEATMAP_LAYER, LayerPosition(null, "building-number-label", null))
                }
            }
        }
    }
    private fun hideHeatmap() {
        mapView.getMapboxMap().getStyle { style ->
            if (style.styleLayerExists(ID_HEATMAP_LAYER)) {
                style.removeStyleLayer(ID_HEATMAP_LAYER)
                style.removeStyleSource(ID_HEATMAP_SOURCE)
            }
        }
    }

    private fun displaySteepness() {
        hideHeatmap()
        hideSteepness()

        val sharedPref = requireActivity().getSharedPreferences(getString(R.string.steepnessFile), Context.MODE_PRIVATE)
        val maxSteepness = sharedPref.getFloat(getString(R.string.maxSteepness), 15F)
        mapView.getMapboxMap().getStyle { style ->
            SteepnessStyle.setStyle(style, ID_STEEPNESS_SOURCE, ID_STEEPNESS_LAYER, maxSteepness)
        }

        val steepnessFile = File(requireContext().filesDir, getString(R.string.steepnessFile))
        if (steepnessFile.exists()) {
            mapView.getMapboxMap().getStyle {
                val geoJsonSource: GeoJsonSource = it.getSourceAs(ID_STEEPNESS_SOURCE)!!
                geoJsonSource.data(steepnessFile.readText(Charsets.UTF_8))

                it.moveStyleLayer(ID_STEEPNESS_LAYER, LayerPosition(null, "building-number-label", null))
            }
        }
    }
    private fun hideSteepness() {
        mapView.getMapboxMap().getStyle { style ->
            if (style.styleLayerExists(ID_STEEPNESS_LAYER)) {
                style.removeStyleLayer(ID_STEEPNESS_LAYER)
                style.removeStyleSource(ID_STEEPNESS_SOURCE)
            }
        }
    }

    private fun displayNavigation(directions: String) {
        mapView.getMapboxMap().getStyle { style ->
            style.addSource(geoJsonSource(ID_NAVIGATION_SOURCE) {
                data(directions)
            })
            style.addLayer(lineLayer(ID_NAVIGATION_LAYER, ID_NAVIGATION_SOURCE) {
                lineCap(LineCap.ROUND)
                lineJoin(LineJoin.ROUND)
                lineOpacity(0.7)
                lineWidth(8.0)
                lineColor("#303F9F")
            })
        }

        val response = Gson().fromJson(directions, NavigationResponse::class.java)
        val coordinatesFrom = response.features[0].properties.start.split(", ")
        val coordinatesTo = response.features[0].properties.end.split(", ")
        val longitudeDelta = coordinatesTo[0].toDouble() - coordinatesFrom[0].toDouble()
        val latitudeDelta = coordinatesTo[1].toDouble() - coordinatesFrom[1].toDouble()
        val bearing = 90.0 + when {
            longitudeDelta == 0.0 -> {
                90 * kotlin.math.sign(latitudeDelta)
            }
            longitudeDelta < 0 -> {
                kotlin.math.atan(latitudeDelta / longitudeDelta) / kotlin.math.PI * 180
            }
            else -> {
                kotlin.math.atan(-latitudeDelta / -longitudeDelta) / kotlin.math.PI * 180
            }
        }

        mapView.getMapboxMap().easeTo(
            CameraOptions.Builder()
                .center(Point.fromLngLat(coordinatesFrom[0].toDouble(), coordinatesFrom[1].toDouble()))
                .bearing(bearing)
                .zoom(14.0)
                .build(),
            mapAnimationOptions {
                duration(4000)
            })

        stopNavigation.isEnabled = true
    }
    fun hideNavigation() {
        mapView.getMapboxMap().getStyle { style ->
            if (style.styleLayerExists(ID_NAVIGATION_LAYER)) {
                style.removeStyleLayer(ID_NAVIGATION_LAYER)
                style.removeStyleSource(ID_NAVIGATION_SOURCE)
            }
        }
        stopNavigation.isEnabled = false
        requireArguments().remove("directions")
    }

    companion object {
        private const val ID_HEATMAP_SOURCE = "heatmap_image"
        private const val ID_HEATMAP_LAYER = "heatmap_layer"
        private const val ID_NAVIGATION_SOURCE = "navigation_source"
        private const val ID_NAVIGATION_LAYER = "navigation_layer"
        private const val ID_STEEPNESS_SOURCE = "steepness_image"
        private const val ID_STEEPNESS_LAYER = "steepness_layer"
        private val INITIAL_CENTER = Point.fromLngLat(6.184376, 62.473821)
        private const val INITIAL_ZOOM = 11.0
    }
}