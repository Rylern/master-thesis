package com.example.walkability

import android.content.Context
import android.content.Intent
import android.os.Bundle
import android.view.MenuItem
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.widget.SwitchCompat
import androidx.appcompat.widget.Toolbar
import androidx.core.app.ActivityCompat
import androidx.drawerlayout.widget.DrawerLayout
import androidx.navigation.NavController
import androidx.navigation.fragment.NavHostFragment
import androidx.navigation.ui.*
import com.example.walkability.fragments.MapFragment
import com.google.android.material.navigation.NavigationView
import java.io.File
import java.util.*


class MainActivity : AppCompatActivity() {
    private lateinit var appBarConfiguration : AppBarConfiguration
    private lateinit var navController : NavController
    private lateinit var navHost : NavHostFragment
    lateinit var stopNavigation: MenuItem
    lateinit var toggleIndicators: SwitchCompat
    lateinit var toggleSteepness: SwitchCompat
    lateinit var toggleLegend: SwitchCompat

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        val toolbar = findViewById<Toolbar>(R.id.toolbar)
        setSupportActionBar(toolbar)

        navHost = supportFragmentManager.findFragmentById(R.id.nav_host_fragment) as NavHostFragment
        val drawerLayout = findViewById<DrawerLayout>(R.id.drawer_layout)
        navController = navHost.navController
        appBarConfiguration = AppBarConfiguration(navController.graph, drawerLayout)
        val navigationView = findViewById<NavigationView>(R.id.nav_view)

        setupActionBarWithNavController(navController, appBarConfiguration)
        navigationView.setupWithNavController(navController)

        navigationView.setNavigationItemSelectedListener{ item ->
            when (item.itemId) {
                R.id.quitApp -> {
                    finishAffinity()
                }
                R.id.toggleLegend -> {}
                R.id.stopNavigation -> {
                    drawerLayout.close()
                    (navHost.childFragmentManager.fragments[0] as MapFragment).hideNavigation()
                }
                else -> {
                    NavigationUI.onNavDestinationSelected(item, navController)
                    drawerLayout.close()
                }
            }
            false
        }

        stopNavigation = navigationView.menu.findItem(R.id.stopNavigation)
        toggleIndicators = navigationView.menu.findItem(R.id.toggleIndicators).actionView as SwitchCompat
        toggleSteepness = navigationView.menu.findItem(R.id.toggleSteepness).actionView as SwitchCompat
        toggleLegend = navigationView.menu.findItem(R.id.toggleLegend).actionView as SwitchCompat

        checkLanguageChange()
        launchInitializationIfIndicatorsNotFound()
    }

    fun updateHeatmap() {
        if (toggleIndicators.isChecked) {
            (navHost.childFragmentManager.fragments[0] as MapFragment).displayHeatmap()
        }
    }

    override fun onOptionsItemSelected(item: MenuItem): Boolean {
        return item.onNavDestinationSelected(navController) || super.onOptionsItemSelected(item)
    }

    override fun onSupportNavigateUp(): Boolean {
        return navController.navigateUp(appBarConfiguration) || super.onSupportNavigateUp()
    }

    private fun checkLanguageChange() {
        val sharedPrefSettings = getSharedPreferences(getString(R.string.settingsFile), Context.MODE_PRIVATE)
        val language = sharedPrefSettings.getString(getString(R.string.language), "")
        if (language != Locale.getDefault().language) {
            with (sharedPrefSettings?.edit()) {
                this?.clear()
                this?.commit()
            }
            File(filesDir, getString(R.string.indicatorsFile)).delete()
        }
    }

    private fun launchInitializationIfIndicatorsNotFound() {
        if (!File(filesDir, getString(R.string.indicatorsFile)).exists()) {
            val intent = Intent(this, InitActivity::class.java)
            intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK)
            startActivity(intent)
            ActivityCompat.finishAffinity(this@MainActivity)
        }
    }

    override fun onRequestPermissionsResult(requestCode: Int, permissions: Array<String>, grantResults: IntArray) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults)
        (navHost.childFragmentManager.fragments[0] as MapFragment).permissionResult(requestCode, permissions, grantResults)
    }
}