﻿/**
 * FlightComputerWindow.cs
 * 
 * Thunder Aerospace Corporation's Flight Computer for the Kerbal Space Program, by Taranis Elsu
 * 
 * (C) Copyright 2013, Taranis Elsu
 * 
 * Kerbal Space Program is Copyright (C) 2013 Squad. See http://kerbalspaceprogram.com/. This
 * project is in no way associated with nor endorsed by Squad.
 * 
 * This code is licensed under the Apache License Version 2.0. See the LICENSE.txt and NOTICE.txt
 * files for more information.
 * 
 * Note that Thunder Aerospace Corporation is a ficticious entity created for entertainment
 * purposes. It is in no way meant to represent a real entity. Any similarity to a real entity
 * is purely coincidental.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Tac
{
    class FlightComputerWindow : Window<FlightComputerWindow>
    {
        private float lastUpdateTime = 0.0f;
        private float updateInterval = 0.1f;

        private GUIStyle labelStyle = null;
        private GUIStyle valueStyle = null;

        private string apoapsis = "";
        private string periapsis = "";
        private string timeToApoapsis = "";
        private string timeToPeriapsis = "";
        private string period = "";
        private string inclination = "";
        private string altitudeAboveSeaLevel = "";
        private string altitudeAboveTerrain = "";

        public FlightComputerWindow()
            : base("TAC Flight Computer", 200, 200)
        {
        }

        protected override void ConfigureStyles()
        {
            base.ConfigureStyles();

            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(GUI.skin.label);
                labelStyle.fontStyle = FontStyle.Normal;
                labelStyle.normal.textColor = Color.white;
                labelStyle.margin.top = 0;
                labelStyle.margin.bottom = 0;
                labelStyle.padding.top = 0;
                labelStyle.padding.bottom = 1;
                labelStyle.wordWrap = false;

                valueStyle = new GUIStyle(labelStyle);
                valueStyle.alignment = TextAnchor.MiddleRight;
                valueStyle.stretchWidth = true;
            }
        }

        protected override void DrawWindowContents(int windowId)
        {
            float now = Time.time;
            if ((now - lastUpdateTime) > updateInterval)
            {
                lastUpdateTime = now;
                UpdateValues();
            }

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            GUILayout.Label("Apoapsis", labelStyle);
            GUILayout.Label("Periapsis", labelStyle);
            GUILayout.Label("Time to Apoapsis", labelStyle);
            GUILayout.Label("Time to Periapsis", labelStyle);
            GUILayout.Space(5.0f);
            GUILayout.Label("Period", labelStyle);
            GUILayout.Label("Inclination", labelStyle);
            GUILayout.Space(5.0f);
            GUILayout.Label("Altitude (sea level)", labelStyle);
            GUILayout.Label("Altitude (terrain)", labelStyle);
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Label(apoapsis, valueStyle);
            GUILayout.Label(periapsis, valueStyle);
            GUILayout.Label(timeToApoapsis, valueStyle);
            GUILayout.Label(timeToPeriapsis, valueStyle);
            GUILayout.Space(5.0f);
            GUILayout.Label(period, valueStyle);
            GUILayout.Label(inclination, valueStyle);
            GUILayout.Space(5.0f);
            GUILayout.Label(altitudeAboveSeaLevel, valueStyle);
            GUILayout.Label(altitudeAboveTerrain, valueStyle);
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

        }

        private void UpdateValues()
        {
            if (FlightGlobals.ready && FlightGlobals.fetch.activeVessel != null)
            {
                Orbit orbit = FlightGlobals.fetch.activeVessel.orbit;

                apoapsis = FormatDistance(orbit.ApA);
                periapsis = FormatDistance(orbit.PeA);
                timeToApoapsis = FormatTime(orbit.timeToAp);
                timeToPeriapsis = FormatTime(orbit.timeToPe);
                period = FormatTime(orbit.period);
                inclination = orbit.inclination.ToString("0.00") + "°";
                altitudeAboveSeaLevel = FormatDistance(orbit.altitude);

                Vessel vessel = FlightGlobals.fetch.activeVessel;
                altitudeAboveTerrain = FormatDistance(orbit.altitude - vessel.terrainAltitude);
            }
        }

        private string FormatDistance(double value)
        {
            string sign = "";
            if (value < 0.0)
            {
                sign = "-";
                value = -value;
            }

            if (value > 1000000.0)
            {
                return sign + (value / 1000000.0).ToString("0.00") + " Mm";
            }
            if (value > 1000.0)
            {
                return sign + (value / 1000.0).ToString("0.00") + " km";
            }
            else
            {
                return sign + value.ToString("#,##0.0") + " m";
            }
        }

        private string FormatTime(double value)
        {
            const double SECONDS_PER_MINUTE = 60.0;
            const double MINUTES_PER_HOUR = 60.0;
            const double HOURS_PER_DAY = 24.0;

            double seconds = value;

            long minutes = (long)(seconds / SECONDS_PER_MINUTE);
            seconds -= (long)(minutes * SECONDS_PER_MINUTE);

            long hours = (long)(minutes / MINUTES_PER_HOUR);
            minutes -= (long)(hours * MINUTES_PER_HOUR);

            long days = (long)(hours / HOURS_PER_DAY);
            hours -= (long)(days * HOURS_PER_DAY);

            if (days > 0)
            {
                return days.ToString("#0") + ":"
                    + hours.ToString("00") + ":"
                    + minutes.ToString("00") + ":"
                    + seconds.ToString("00");
            }
            else if (hours > 0)
            {
                return hours.ToString("#0") + ":"
                    + minutes.ToString("00") + ":"
                    + seconds.ToString("00");
            }
            else
            {
                return minutes.ToString("#0") + ":"
                    + seconds.ToString("00.0");
            }
        }
    }
}
