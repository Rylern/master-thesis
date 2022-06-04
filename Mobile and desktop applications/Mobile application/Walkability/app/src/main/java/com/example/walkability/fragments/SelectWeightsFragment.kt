package com.example.walkability.fragments

import android.content.Context
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.LinearLayout
import android.widget.TextView
import androidx.core.view.children
import androidx.fragment.app.Fragment
import androidx.navigation.findNavController
import com.example.walkability.R
import com.example.walkability.models.Indicator
import com.google.android.material.slider.Slider
import java.io.*
import java.util.*


class SelectWeightsFragment : Fragment() {

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        return inflater.inflate(R.layout.fragment_select_weights, container, false)
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        val indicatorFile = File(requireContext().filesDir, getString(R.string.indicatorsFile))
        var indicators: List<Indicator> = listOf()
        if (indicatorFile.exists()) {
            indicators = ObjectInputStream(FileInputStream(indicatorFile)).readObject() as List<Indicator>
        }

        val weightList = view.findViewById<LinearLayout>(R.id.weightList)
        indicators.forEach {
            val weightView = LayoutInflater.from(activity).inflate(R.layout.indicator_weight, weightList, false)
            weightView.findViewById<TextView>(R.id.indicator).text = it.name
            weightView.findViewById<Slider>(R.id.uphillSteepnessSlider).value = it.weight
            weightList.addView(weightView)
        }

        val submit: Button = view.findViewById(R.id.submit)
        submit.setOnClickListener {
            weightList.children.forEach {
                val indicator = indicators.find { indicator -> indicator.name ==  it.findViewById<TextView>(R.id.indicator).text as String }
                if (indicator != null) {
                    indicator.weight = it.findViewById<Slider>(R.id.uphillSteepnessSlider).value
                }
            }
            ObjectOutputStream(FileOutputStream(File(requireContext().filesDir, getString(R.string.indicatorsFile)))).writeObject(indicators)

            val sharedPrefSettings = activity?.getSharedPreferences(getString(R.string.settingsFile), Context.MODE_PRIVATE)
            with (sharedPrefSettings?.edit()) {
                this?.putString(getString(R.string.language), Locale.getDefault().language)
                this?.commit()
            }

            view.findNavController().navigate(R.id.loadingFragment)
        }
    }
}