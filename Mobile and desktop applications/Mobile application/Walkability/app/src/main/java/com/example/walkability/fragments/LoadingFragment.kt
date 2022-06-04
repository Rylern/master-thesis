package com.example.walkability.fragments

import android.content.Intent
import android.os.Bundle
import android.util.Log
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast
import androidx.navigation.findNavController
import androidx.navigation.fragment.navArgs
import com.example.walkability.MainActivity
import com.example.walkability.R
import com.example.walkability.api.DataProvider
import com.example.walkability.models.Indicator
import com.google.gson.Gson
import kotlinx.coroutines.*
import java.io.File
import java.io.FileInputStream
import java.io.ObjectInputStream

class LoadingFragment : Fragment() {
    private val args: LoadingFragmentArgs by navArgs()
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
        return inflater.inflate(R.layout.fragment_loading, container, false)
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        fetchHeatmap {
            if (args.init) {
                fetchSteepness {
                    startActivity(Intent(activity, MainActivity::class.java))
                }
            } else {
                view.findNavController().navigate(R.id.mapFragment)
            }
        }
    }

    override fun onStop() {
        super.onStop()
        activityScope.cancel()
    }

    private fun fetchHeatmap(onFinished: () -> Unit) {
        val indicatorFile = File(requireContext().filesDir, getString(R.string.indicatorsFile))
        var indicators: List<Indicator> = listOf()
        if (indicatorFile.exists()) {
            indicators = ObjectInputStream(FileInputStream(indicatorFile)).readObject() as List<Indicator>
        }

        activityScope.launch {
            kotlin.runCatching {
                DataProvider.getHeatmap(indicators)
            }.fold(
                onSuccess = { response ->
                    val geoJSON = Gson().toJson(response)
                    File(requireContext().filesDir, getString(R.string.heatmapFile)).writeText(geoJSON)
                    onFinished()
                },
                onFailure = {
                    try {
                        Toast.makeText(context, R.string.connection_failed, Toast.LENGTH_SHORT).show()
                        onFinished()
                    } catch (e: Throwable) {}
                }
            )
        }
    }

    private fun fetchSteepness(onFinished: () -> Unit) {
        activityScope.launch {
            kotlin.runCatching {
                DataProvider.getSteepness()
            }.fold(
                onSuccess = { response ->
                    val geoJSON = Gson().toJson(response)
                    File(requireContext().filesDir, getString(R.string.steepnessFile)).writeText(geoJSON)
                    onFinished()
                },
                onFailure = {
                    Toast.makeText(context, R.string.connection_failed, Toast.LENGTH_SHORT).show()
                    onFinished()
                }
            )
        }
    }
}