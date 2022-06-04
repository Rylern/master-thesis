package com.example.walkability.fragments

import android.annotation.SuppressLint
import android.content.Context
import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.RadioButton
import android.widget.TextView
import android.widget.Toast
import androidx.navigation.findNavController
import com.example.walkability.R
import com.google.android.material.slider.Slider


class SteepnessDefinitionFragment : Fragment() {
    private lateinit var steepnessSlider: Slider
    private lateinit var steepnessLabel: TextView

    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?): View? {
        return inflater.inflate(R.layout.fragment_steepness_definition, container, false)
    }

    @SuppressLint("SetTextI18n")
    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        steepnessSlider = view.findViewById(R.id.uphillSteepnessSlider)
        steepnessLabel = view.findViewById(R.id.uphillSteepnessLabel)

        val sharedPref = requireActivity().getSharedPreferences(getString(R.string.steepnessFile), Context.MODE_PRIVATE)
        val maxSteepness = sharedPref.getFloat(getString(R.string.maxSteepness), -1F)
        if (maxSteepness != -1F) {
            steepnessSlider.value = maxSteepness
        }

        steepnessLabel.text = String.format("%.1f", steepnessSlider.value) + "%"
        steepnessSlider.addOnChangeListener { _, value, _ ->
            steepnessLabel.text = String.format("%.1f", value) + "°"
        }

        view.findViewById<RadioButton>(R.id.custom).setOnClickListener {
            steepnessSlider.isEnabled = true
        }
        view.findViewById<RadioButton>(R.id.wheelchair).setOnClickListener {
            steepnessSlider.value = 5.0f
            steepnessSlider.isEnabled = false
        }
        view.findViewById<RadioButton>(R.id.powered).setOnClickListener {
            steepnessSlider.value = 7.0f
            steepnessSlider.isEnabled = false
        }
        view.findViewById<RadioButton>(R.id.cane).setOnClickListener {
            steepnessSlider.value = 8.0f
            steepnessSlider.isEnabled = false
        }

        view.findViewById<Button>(R.id.submit).setOnClickListener {
            with (sharedPref?.edit()) {
                this?.putFloat(getString(R.string.maxSteepness), steepnessSlider.value)
                this?.apply()
            }

            Toast.makeText(activity, R.string.steepnessDefined, Toast.LENGTH_LONG).show()
            view.findNavController().navigate(R.id.mapFragment)
        }
    }
}