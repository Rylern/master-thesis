package com.example.walkability.fragments

import android.app.AlertDialog
import android.app.Dialog
import android.content.Context
import android.os.Bundle
import android.widget.ArrayAdapter
import android.widget.Spinner
import android.widget.Toast
import androidx.fragment.app.DialogFragment
import com.example.walkability.MainActivity
import com.example.walkability.R
import java.io.*

class LoadIndicatorsFragment : DialogFragment() {
    override fun onCreateDialog(savedInstanceState: Bundle?): Dialog {
        return activity?.let { fragmentActivity ->
            val builder = AlertDialog.Builder(fragmentActivity)
            val inflater = requireActivity().layoutInflater
            val view = inflater.inflate(R.layout.dialog_load, null)
            val fileName = view.findViewById<Spinner>(R.id.fileName)

            val fileList = requireContext().getDir(getString(R.string.indicators), Context.MODE_PRIVATE).listFiles().map { it.name }
            fileName.adapter = ArrayAdapter(requireContext(), android.R.layout.simple_spinner_item, fileList)

            builder
                .setMessage(getString(R.string.loadIndicators))
                .setView(view)
                .setPositiveButton(R.string.load)
                { _, _ ->
                    if (fileName.selectedItem != null) {
                        val directory = requireContext().getDir(getString(R.string.indicators), Context.MODE_PRIVATE)
                        val savedFile = File(directory, fileName.selectedItem.toString())
                        val heatmapFile = File(requireContext().filesDir, getString(R.string.heatmapFile))

                        if (savedFile.exists()) {
                            savedFile.copyTo(heatmapFile, true)
                        }

                        (requireActivity() as MainActivity).updateHeatmap()
                        Toast.makeText(activity, R.string.indicatorsLoaded, Toast.LENGTH_SHORT).show()
                    }
                }
                .setNegativeButton(R.string.cancel) { _, _ -> }
            builder.create()
        } ?: throw IllegalStateException("Activity cannot be null")
    }
}