package com.example.walkability.fragments

import android.app.AlertDialog
import android.app.Dialog
import android.content.Context
import android.os.Bundle
import android.util.Log
import android.view.View
import android.widget.EditText
import android.widget.ProgressBar
import android.widget.Toast
import androidx.fragment.app.DialogFragment
import androidx.preference.PreferenceManager
import com.example.walkability.R
import com.example.walkability.api.DataProvider
import com.example.walkability.models.Indicator
import kotlinx.coroutines.*
import java.io.*
import java.util.*


class SaveIndicatorsFragment : DialogFragment() {
    private val activityScope = CoroutineScope(
        SupervisorJob()
                + Dispatchers.Main
                + CoroutineExceptionHandler { _, throwable ->
            Log.e("azerty", "Coroutine exception : ", throwable)
        }
    )

    override fun onCreateDialog(savedInstanceState: Bundle?): Dialog {
        return activity?.let { fragmentActivity ->
            val builder = AlertDialog.Builder(fragmentActivity)
            val inflater = requireActivity().layoutInflater
            val view = inflater.inflate(R.layout.dialog_save, null)
            builder
                .setMessage(getString(R.string.saveCurrentIndicators))
                .setView(view)
                .setPositiveButton(R.string.save)
                { _, _ ->
                    val fileName = view.findViewById<EditText>(R.id.fileName).text.toString()
                    if (fileName != "") {
                        val directory = requireContext().getDir(getString(R.string.indicators), Context.MODE_PRIVATE)

                        val heatmapFile = File(requireContext().filesDir, getString(R.string.heatmapFile))
                        if (heatmapFile.exists()) {
                            heatmapFile.copyTo(File(directory, fileName), true)
                        }

                        val indicatorFile = File(requireContext().filesDir, getString(R.string.indicatorsFile))
                        var indicators: List<Indicator> = listOf()
                        if (indicatorFile.exists()) {
                            indicators = ObjectInputStream(FileInputStream(indicatorFile)).readObject() as List<Indicator>
                        }
                        activityScope.launch {
                            kotlin.runCatching {
                                DataProvider.saveHeatmap(indicators)
                            }.fold(
                                onSuccess = {},
                                onFailure = {}
                            )
                        }

                        Toast.makeText(activity, R.string.indicatorsSaved, Toast.LENGTH_LONG).show()
                    } else {
                        Toast.makeText(activity, R.string.specifyFileName, Toast.LENGTH_LONG).show()
                    }
                }
                .setNegativeButton(R.string.cancel) { _, _ -> }
            builder.create()
        } ?: throw IllegalStateException("Activity cannot be null")
    }
}