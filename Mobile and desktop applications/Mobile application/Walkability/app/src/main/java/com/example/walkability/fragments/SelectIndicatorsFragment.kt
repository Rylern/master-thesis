package com.example.walkability.fragments

import android.os.Bundle
import android.util.Log
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.*
import androidx.core.view.children
import androidx.core.view.isVisible
import androidx.navigation.findNavController
import androidx.navigation.fragment.navArgs
import androidx.preference.PreferenceManager
import com.example.walkability.R
import com.example.walkability.api.DataProvider
import com.example.walkability.models.Indicator
import kotlinx.coroutines.*
import java.io.*
import java.util.*

class SelectIndicatorsFragment : Fragment() {
    private val args: SelectWeightsFragmentArgs by navArgs()
    private lateinit var indicators: Array<Indicator>
    private lateinit var description: String
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
        return inflater.inflate(R.layout.fragment_select_indicators, container, false)
    }

    override fun onResume() {
        super.onResume()

        activityScope.launch {
            kotlin.runCatching {
                DataProvider.getIndicators(Locale.getDefault().language, PreferenceManager.getDefaultSharedPreferences(requireContext()).getBoolean("advancedMode", true))
            }.fold(
                onSuccess = { response ->
                    indicators = response.indicators.map {
                        if (it.parameterLabel != null && it.defaultParameter != null) {
                            Indicator(name = it.name, description = it.description, group = it.group, datasetName = it.datasetName, parameterLabel = it.parameterLabel, defaultParameterValue = it.defaultParameter)
                        } else {
                            Indicator(name = it.name, description = it.description, group = it.group, datasetName = it.datasetName)
                        }
                    }.toTypedArray()
                    description = response.description
                    updateUI()
                },
                onFailure = {
                    view?.findViewById<ProgressBar>(R.id.progressBar)?.visibility = View.GONE
                    Toast.makeText(activity, R.string.connection_failed, Toast.LENGTH_SHORT).show()
                }
            )
        }
    }

    private fun updateUI() {
        val view = requireView()

        view.findViewById<ProgressBar>(R.id.progressBar).visibility = View.GONE
        view.findViewById<TextView>(R.id.description).text = description

        val indicatorFile = File(requireContext().filesDir, getString(R.string.indicatorsFile))
        var indicatorsFromFile: List<Indicator> = listOf()
        if (indicatorFile.exists()) {
            indicatorsFromFile = ObjectInputStream(FileInputStream(indicatorFile)).readObject() as List<Indicator>
        }

        val groupList = view.findViewById<LinearLayout>(R.id.indicatorList)
        groupList.removeAllViews()
        indicators
            .map { it.group }
            .distinct()
            .forEach {
                val groupView = LayoutInflater.from(activity).inflate(R.layout.indicator_group, groupList, false)
                groupView.findViewById<TextView>(R.id.name).text = it
                groupList.addView(groupView)
            }

        indicators.forEach { indicator ->
            groupList.children
                .filter {
                    it.findViewById<TextView>(R.id.name).text == indicator.group
                }
                .forEach { groupView ->
                    val indicatorList = groupView.findViewById<LinearLayout>(R.id.indicators)
                    val indicatorView = LayoutInflater.from(activity).inflate(R.layout.indicator_choice, indicatorList, false)
                    indicatorView.findViewById<TextView>(R.id.name).text = indicator.name
                    indicatorView.findViewById<TextView>(R.id.description).text = indicator.description

                    val parameterValue = indicatorView.findViewById<EditText>(R.id.parameterValue)
                    if (indicator.parameterLabel != "") {
                        indicatorView.findViewById<LinearLayout>(R.id.parameter).isVisible = true
                        indicatorView.findViewById<TextView>(R.id.parameterLabel).text = indicator.parameterLabel
                        parameterValue.setText(indicator.defaultParameterValue)
                    }

                    if (args.init) {
                        indicatorView.findViewById<CheckBox>(R.id.active).isChecked = true
                    } else {
                        val indicatorFromFile = indicatorsFromFile.find { it.datasetName == indicator.datasetName }
                        if (indicatorFromFile != null && indicatorFromFile.parameterValue >= 0) {
                            parameterValue.setText(indicatorFromFile.parameterValue.toString())
                        }
                        indicatorView.findViewById<CheckBox>(R.id.active).isChecked = indicatorFromFile != null
                    }
                    indicatorList.addView(indicatorView)
                }
        }

        val submit: Button = view.findViewById(R.id.submit)
        submit.setOnClickListener {
            var atLeastOneSelected = false

            val newIndicators = mutableListOf<Indicator>()
            groupList.children.forEach {
                it.findViewById<LinearLayout>(R.id.indicators).children.forEach { indicatorView ->
                    if (indicatorView.findViewById<CheckBox>(R.id.active).isChecked) {
                        atLeastOneSelected = true

                        val name = indicatorView.findViewById<TextView>(R.id.name).text as String
                        val indicator = indicators.find { indicator -> indicator.name == name }

                        if (indicator != null) {
                            if (indicatorView.findViewById<LinearLayout>(R.id.parameter).isVisible) {
                                indicator.parameterValue = indicatorView.findViewById<EditText>(R.id.parameterValue).text.toString().toFloat()
                            }
                            newIndicators.add(indicator)
                        }
                    }
                }
            }
            ObjectOutputStream(FileOutputStream(File(requireContext().filesDir, getString(R.string.indicatorsFile)))).writeObject(newIndicators)

            if (atLeastOneSelected) {
                view.findNavController().navigate(R.id.selectWeightsFragment)
            } else {
                Toast.makeText(activity, R.string.selectAtLeastOneIndicator, Toast.LENGTH_LONG).show()
            }
        }
    }
}