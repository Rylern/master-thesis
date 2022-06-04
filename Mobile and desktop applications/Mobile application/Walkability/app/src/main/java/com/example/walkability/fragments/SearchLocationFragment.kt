package com.example.walkability.fragments

import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.*
import androidx.core.os.bundleOf
import androidx.fragment.app.Fragment
import androidx.navigation.findNavController
import com.example.walkability.R
import com.example.walkability.api.DataProvider
import com.example.walkability.models.Indicator
import com.google.android.gms.location.FusedLocationProviderClient
import com.google.android.gms.location.LocationServices
import com.google.gson.Gson
import com.mapbox.geojson.Point
import com.mapbox.search.*
import com.mapbox.search.result.SearchResult
import com.mapbox.search.result.SearchSuggestion
import kotlinx.coroutines.*
import retrofit2.HttpException
import java.io.File
import java.io.FileInputStream
import java.io.ObjectInputStream


class SearchLocationFragment : Fragment() {
    private lateinit var reverseGeocodingSearchEngine: ReverseGeocodingSearchEngine
    private lateinit var reverseGeocodingSearchRequestTask: SearchRequestTask
    private lateinit var fusedLocationClient: FusedLocationProviderClient
    private lateinit var searchEngine: SearchEngine
    private lateinit var searchRequestTask: SearchRequestTask
    private lateinit var search: Button
    private lateinit var progressBar: ProgressBar
    private val activityScope = CoroutineScope(
        SupervisorJob()
                + Dispatchers.Main
                + CoroutineExceptionHandler { _, throwable ->
            Log.e("azerty", "Coroutine exception : ", throwable)
        }
    )

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        return inflater.inflate(R.layout.fragment_search_location, container, false)
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        reverseGeocodingSearchEngine = MapboxSearchSdk.getReverseGeocodingSearchEngine()
        fusedLocationClient = LocationServices.getFusedLocationProviderClient(requireActivity())
        searchEngine = MapboxSearchSdk.getSearchEngine()

        val fromLocation = view.findViewById<EditText>(R.id.fromLocation)
        val toLocation = view.findViewById<EditText>(R.id.toLocation)

        view.findViewById<Button>(R.id.fromMyLocation).setOnClickListener {
            onMyLocationPressed(fromLocation)
        }
        view.findViewById<Button>(R.id.toMyLocation).setOnClickListener {
            onMyLocationPressed(toLocation)
        }

        progressBar = view.findViewById(R.id.progressBar)
        search = view.findViewById(R.id.search)
        search.setOnClickListener {
            searchLocation(fromLocation) { searchResult ->
                val from = searchResult.coordinate
                searchLocation(toLocation) {
                    val to = it.coordinate
                    if (from != null && to != null) {
                        requestNavigation(from, to)
                    } else {
                        Toast.makeText(requireActivity(), R.string.unknownLocation, Toast.LENGTH_SHORT).show()
                    }
                }
            }
        }
    }

    private fun requestNavigation(from: Point, to: Point) {
        val indicatorFile = File(requireContext().filesDir, getString(R.string.indicatorsFile))
        var indicators: List<Indicator> = listOf()
        if (indicatorFile.exists()) {
            indicators = ObjectInputStream(FileInputStream(indicatorFile)).readObject() as List<Indicator>
        }

        activityScope.launch {
            kotlin.runCatching {
                search.isEnabled = false
                progressBar.visibility = View.VISIBLE
                DataProvider.getNavigation(indicators, from, to)
            }.fold(
                onSuccess = { response ->
                    search.isEnabled = true
                    progressBar.visibility = View.INVISIBLE

                    val bundle = bundleOf("directions" to Gson().toJson(response))
                    requireView().findNavController().navigate(R.id.mapFragment, bundle)
                },
                onFailure = {
                    search.isEnabled = true
                    progressBar.visibility = View.INVISIBLE
                    when (it) {
                        is HttpException -> {
                            if (it.code() == 404) {
                                Toast.makeText(activity, R.string.routeNotFound, Toast.LENGTH_LONG).show()
                            }
                        } else -> {
                        Toast.makeText(activity, R.string.connection_failed, Toast.LENGTH_LONG).show()
                        }
                    }
                }
            )
        }
    }

    override fun onDestroy() {
        if (::reverseGeocodingSearchRequestTask.isInitialized) {
            reverseGeocodingSearchRequestTask.cancel()
        }
        if (::searchRequestTask.isInitialized) {
            searchRequestTask.cancel()
        }
        super.onDestroy()
    }

    private fun searchLocation(editText: EditText, processResult: (result: SearchResult) -> Unit) {
        searchRequestTask = searchEngine.search(
            editText.text.toString(),
            SearchOptions(limit = 1),
            object : SearchSelectionCallback {
                override fun onSuggestions(suggestions: List<SearchSuggestion>, responseInfo: ResponseInfo) {
                    if (suggestions.isEmpty()) {
                        editText.error = getString(R.string.unknownLocation)
                    } else {
                        searchRequestTask = searchEngine.select(suggestions.first(), this)
                    }
                }
                override fun onResult(suggestion: SearchSuggestion, result: SearchResult, responseInfo: ResponseInfo) {
                    processResult.invoke(result)
                }
                override fun onCategoryResult(suggestion: SearchSuggestion, results: List<SearchResult>, responseInfo: ResponseInfo) {}
                override fun onError(e: Exception) {
                    editText.error = getString(R.string.unknownLocation)
                }
            }
        )
    }

    private fun onMyLocationPressed(textView: EditText) {
        try {
            fusedLocationClient.lastLocation
                .addOnSuccessListener {
                    if (it != null) {
                        reverseGeocodingSearchRequestTask = reverseGeocodingSearchEngine.search(
                            ReverseGeoOptions(
                                center = Point.fromLngLat(it.longitude, it.latitude),
                                limit = 1
                            ),
                            object : SearchCallback {
                                override fun onResults(results: List<SearchResult>, responseInfo: ResponseInfo) {
                                    if (results.isEmpty()) {
                                        Toast.makeText(requireActivity(), R.string.unknownLocation, Toast.LENGTH_SHORT).show()
                                    } else {
                                        val text = when (results[0].address) {
                                            null -> results[0].name
                                            else -> {
                                                results[0].address?.houseNumber + " " + results[0].address?.street + " " + results[0].address?.place + " "
                                            }
                                        }
                                        textView.setText(text)
                                        textView.error = null
                                    }
                                }
                                override fun onError(e: Exception) {
                                    Toast.makeText(requireActivity(), R.string.unknownLocation, Toast.LENGTH_SHORT).show()
                                }
                            }
                        )
                    } else {
                        Toast.makeText(requireActivity(), R.string.notRetrieveLocation, Toast.LENGTH_SHORT).show()
                    }
                }
                .addOnFailureListener {
                    Toast.makeText(requireActivity(), R.string.notRetrieveLocation, Toast.LENGTH_SHORT).show()
                }
        } catch (e: SecurityException) {
            Toast.makeText(requireActivity(), R.string.locationNotEnabled, Toast.LENGTH_SHORT).show()
        }
    }
}