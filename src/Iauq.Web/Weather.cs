using System;
using System.Collections;
using System.Xml;

namespace Iauq.Web
{
    /// <summary>
    /// Class to manage Yahoo! Weather Forecast information
    /// </summary>
    public class Weather
    {
        /// <summary>
        /// The Yahoo! Location ID used to get localized weather information
        /// To find you Location ID, open http://weather.yahoo.com/ in a web
        /// browser, search or browse for your city and copy the number from
        /// the URL in the address bar.  (Example CAXX0051)
        /// </summary>
        public string LocationId = "IRXX0008";

        private string baseUrl = "http://weather.yahooapis.com/forecastrss?";

        private bool isMetric = false;
        /// <summary>
        /// Get wether forecast is in metric or standard
        /// </summary>
        public bool IsMetric
        {
            get { return isMetric; }
        }

        /// <summary>
        /// Enumeration of temperature units
        /// </summary>
        public enum TemperatureUnits
        {
            Celcius,
            Fahrenheit,
            Null
        }
        /// <summary>
        /// Enumeration of distance units
        /// </summary>
        public enum DistanceUnits
        {
            Miles,
            Kilometeres,
            Null
        }
        /// <summary>
        /// Enumeration of pressure units
        /// </summary>
        public enum PressureUnits
        {
            PoundsPerSquareInch,
            Millibars,
            Null
        }
        /// <summary>
        /// Enumeration of speed units
        /// </summary>
        public enum SpeedUnits
        {
            MilesPerHour,
            KilometersPerHour,
            Null
        }

        /// <summary>
        /// State of the barometric pressure
        /// </summary>
        public enum BarometricPressure
        {
            Steady = 0, Rising = 1, Falling = 2
        }

        /// <summary>
        /// Condition codes enumeration to describe the current conditions
        /// </summary>
        public enum ConditionCodes
        {
            Tornado = 0,
            TropicalStorm = 1,
            Hurricane = 2,
            SeverThunderstorms = 3,
            Thunderstorms = 4,
            MixedRainAndSnow = 5,
            MixedRainAndSleet = 6,
            MixedSnowAndSleet = 7,
            FreezingDrizzle = 8,
            Drizzle = 9,
            FreezingRain = 10,
            Showers1 = 11,
            Showers2 = 12,
            SnowFlurries = 13,
            LightSnowShowers = 14,
            BlowingSnow = 15,
            Snow = 16,
            Hail = 17,
            Sleet = 18,
            Dust = 19,
            Foggy = 20,
            Haze = 21,
            Smoky = 22,
            Blustery = 23,
            Windy = 24,
            Cold = 25,
            Cloudy = 26,
            MostlyCloudyNight = 27,
            MostlyCloudyDay = 28,
            PartlyCloudyNight = 29,
            PartlyCloudyDay = 30,
            ClearNight = 31,
            Sunny = 32,
            FairNight = 33,
            FairDay = 34,
            MixedRainAndHail = 35,
            Hot = 36,
            IsolatedThunderstorms = 37,
            ScatteredThunderstorms1 = 38,
            ScatteredThunderstorms2 = 39,
            ScatteredShowers = 40,
            HeavySnow1 = 41,
            ScatteredSnowShowers = 42,
            HeavySnow2 = 43,
            PartlyCloudy = 44,
            Thundershowers = 45,
            SnowShowers = 46,
            IsolatedThundershowers = 47,
            NotAvailable = 3200
        }


        /// <summary>
        /// The location of this forecast
        /// </summary>
        public Location Location = new Location("", "", "");
        /// <summary>
        /// Units for various aspects of the forecast
        /// </summary>
        public Units Units = new Units(TemperatureUnits.Null, DistanceUnits.Null, PressureUnits.Null, SpeedUnits.Null);
        /// <summary>
        /// Forecast information about wind
        /// </summary>
        public Wind Wind = new Wind(-1, -1, -1);
        /// <summary>
        /// Forecast information about current atmospheric pressure, humidity, and visibility
        /// </summary>
        public Atmosphere Atmosphere = new Atmosphere(-1, -1, -1, 0);
        /// <summary>
        /// Forecast information about current astronomical conditions
        /// </summary>
        public Astronomy Astronomy = new Astronomy(new DateTime(), new DateTime());
        /// <summary>
        /// The latitude and longitude of the location
        /// </summary>
        public Geography Geography = new Geography(-1, -1);
        /// <summary>
        /// The current weather conditions
        /// </summary>
        public Condition Condition = new Condition("", (ConditionCodes)3200, -1, new DateTime());
        /// <summary>
        /// The weather forecast for today and tomorrow
        /// </summary>
        public Forecast Forecast = new Forecast();


        /// <summary>
        /// Default Weather constructor no arguments, uses the default Location ID
        /// </summary>

        /// <summary>
        /// Overloaded Weather constructor
        /// </summary>
        /// <param name="location">The Yahoo! Location ID used to get localized weather information</param>
        public Weather(string location, TemperatureUnits units = TemperatureUnits.Celcius)
        {
            LocationId = location;
            Units.Temperature = units;
            Refresh();
        }

        /// <summary>
        /// Method to retrieve latest forecast from Yahoo! Weather
        /// </summary>
        public void Refresh()
        {
            // track if we are inside a geo:lat tag
            bool isInGeoLattitude = false;

            // track if we are inside a geo:long tag
            bool isInGeoLongitude = false;

            // temporary vars used to hold parsed xml
            DateTime date;
            ConditionCodes code;
            int low, high, rising, conditionCode;
            string text, temp;


            // to hold constructed url
            string forecastUrl;

            // check which units (c/f) are selected and build the url accordingly
            if (Units.Temperature == TemperatureUnits.Celcius)
            {
                forecastUrl = baseUrl + "p=" + LocationId + "&u=c";
                isMetric = true;
            }
            else
            {
                forecastUrl = baseUrl + "p=" + LocationId + "&u=f";
                isMetric = false;
            }

            // clear out the forecast days collection
            Forecast.Days.Clear();

            //catch-all error handling
            try
            {
                // open a XmlTextReader object using the constructed url
                XmlTextReader reader = new XmlTextReader("C:\\forecastrss.xml");//(forecastUrl);
                // loop through xml result node by node
                while (reader.Read())
                {
                    // decide which type of node us currently being read
                    switch (reader.NodeType)
                    {
                        // xml start element
                        case XmlNodeType.Element:
                            // read the tag name and decide which objects to load
                            if (reader.Name.ToLower() == "yweather:location")
                            {
                                Location.City = reader.GetAttribute("city");
                                Location.Region = reader.GetAttribute("region");
                                Location.Country = reader.GetAttribute("country");
                            }
                            if (reader.Name.ToLower() == "yweather:units")
                            {
                                // store in temporary variable
                                temp = reader.GetAttribute("temperature").ToLower();
                                // put it into the correct units
                                if (temp == "c")
                                {
                                    Units.Temperature = TemperatureUnits.Celcius;
                                }
                                else
                                {
                                    Units.Temperature = TemperatureUnits.Fahrenheit;
                                }

                                temp = reader.GetAttribute("distance");
                                if (temp == "km")
                                {
                                    Units.Distance = DistanceUnits.Kilometeres;
                                }
                                else
                                {
                                    Units.Distance = DistanceUnits.Miles;
                                }

                                temp = reader.GetAttribute("pressure");
                                if (temp == "mb")
                                {
                                    Units.Pressure = PressureUnits.Millibars;
                                }
                                else
                                {
                                    Units.Pressure = PressureUnits.PoundsPerSquareInch;
                                }

                                temp = reader.GetAttribute("speed");
                                if (temp == "kph")
                                {
                                    Units.Speed = SpeedUnits.KilometersPerHour;
                                }
                                else
                                {
                                    Units.Speed = SpeedUnits.MilesPerHour;
                                }
                            }
                            if (reader.Name.ToLower() == "yweather:wind")
                            {
                                Wind.Chill = Convert.ToInt32(reader.GetAttribute("chill"));
                                Wind.Direction = Convert.ToInt32(reader.GetAttribute("direction"));
                                Wind.Speed = Convert.ToInt32(reader.GetAttribute("speed"));
                            }
                            if (reader.Name.ToLower() == "yweather:atmosphere")
                            {
                                Atmosphere.Humidity = Convert.ToInt32(reader.GetAttribute("humidity"));
                                Atmosphere.Visibility = Convert.ToDecimal(reader.GetAttribute("visibility"));
                                Atmosphere.Pressure = Convert.ToDouble(reader.GetAttribute("pressure"));
                                rising = Convert.ToInt32(reader.GetAttribute("rising"));
                                Atmosphere.Rising = (BarometricPressure)rising;
                            }
                            if (reader.Name.ToLower() == "yweather:astronomy")
                            {
                                Astronomy.Sunrise = Convert.ToDateTime(reader.GetAttribute("sunrise"));
                                Astronomy.Sunset = Convert.ToDateTime(reader.GetAttribute("sunset"));
                            }
                            if (reader.Name.ToLower() == "yweather:condition")
                            {
                                Condition.Text = reader.GetAttribute("text");
                                conditionCode = Convert.ToInt32(reader.GetAttribute("code"));
                                Condition.Code = (ConditionCodes)conditionCode;
                                Condition.Temperature = Convert.ToInt32(reader.GetAttribute("temp"));
                                Condition.Date = ParseDateTime(reader.GetAttribute("date"));
                            }
                            if (reader.Name.ToLower() == "yweather:forecast")
                            {
                                date = Convert.ToDateTime(reader.GetAttribute("date"));
                                low = Convert.ToInt32(reader.GetAttribute("low"));
                                high = Convert.ToInt32(reader.GetAttribute("high"));
                                text = reader.GetAttribute("text");
                                code = (ConditionCodes)Convert.ToInt32(reader.GetAttribute("code"));
                                Forecast.Days.Add(new ForecastDay(date, low, high, text, code));
                            }
                            // set the flag if we're in the geo:long tag
                            if (reader.Name.ToLower() == "geo:long")
                            {
                                isInGeoLongitude = true;
                            }
                            // set the flag if we're in the geo:lat tag
                            if (reader.Name.ToLower() == "geo:lat")
                            {
                                isInGeoLattitude = true;
                            }
                            break;
                        // xml element text
                        case XmlNodeType.Text:
                            // if we're currently in the geo:lat tag
                            if (isInGeoLattitude)
                            {
                                // read the value from the node
                                Geography.Lattitude = Convert.ToDecimal(reader.Value);
                            }
                            // if we're currently in the geo:long tag
                            if (isInGeoLongitude)
                            {
                                // read the value from the node
                                Geography.Longitude = Convert.ToDecimal(reader.Value);
                            }
                            break;
                        // xml end element
                        case XmlNodeType.EndElement:
                            // clear the flag once we leave the geo:long tag
                            if (reader.Name.ToLower() == "geo:long")
                            {
                                isInGeoLongitude = false;
                            }
                            // clear the flag once we leave the geo:lat tag
                            if (reader.Name.ToLower() == "geo:lat")
                            {
                                isInGeoLattitude = false;
                            }
                            break;
                    }
                }
            }
            // A basic catch-all error handling routine
            catch (Exception ex)
            {
                // On any exception, throw a new exception containing the message
                // of the current exception
                throw new Exception(ex.Message);
            }
        }

        public void ConvertToMetric()
        {
            // c = (f-32)/1.8
            Condition.Temperature = Convert.ToInt32((Condition.Temperature - 32) / 1.8);
            // km = mi * 1.609344
            Atmosphere.Visibility = Convert.ToDecimal(Atmosphere.Visibility * (decimal)1.61);
            // mb = psi * 68.9475729
            Atmosphere.Pressure = Convert.ToDouble(Atmosphere.Pressure * (double)68.9475729);

            Wind.Chill = Convert.ToInt32((Wind.Chill - 32) / 1.8);
            Wind.Speed = Convert.ToInt32(Wind.Speed * 1.609344);

            foreach (ForecastDay fd in Forecast.Days)
            {
                fd.Low = Convert.ToInt32((fd.Low - 32) / 1.8);
                fd.High = Convert.ToInt32((fd.High - 32) / 1.8);
            }

            Units.Temperature = TemperatureUnits.Celcius;
            Units.Distance = DistanceUnits.Kilometeres;
            Units.Pressure = PressureUnits.Millibars;
            Units.Speed = SpeedUnits.KilometersPerHour;
        }

        public void ConvertToStandard()
        {
            // (f+32)*1.8
            Condition.Temperature = Convert.ToInt32((Condition.Temperature + 32) * 1.8);
            // km * 0.621371192 = mi
            Atmosphere.Visibility = Convert.ToDecimal(Atmosphere.Visibility * (decimal)0.62);
            // psi = mb * 0.0145037738
            Atmosphere.Pressure = Convert.ToDouble(Atmosphere.Pressure * (double)0.0145037738);

            Wind.Chill = Convert.ToInt32((Wind.Chill + 32) * 1.8);
            Wind.Speed = Convert.ToInt32(Wind.Speed * 0.621371192);

            foreach (ForecastDay fd in Forecast.Days)
            {
                fd.Low = Convert.ToInt32((fd.Low + 32) * 1.8);
                fd.High = Convert.ToInt32((fd.High + 32) * 1.8);
            }

            Units.Temperature = TemperatureUnits.Fahrenheit;
            Units.Distance = DistanceUnits.Miles;
            Units.Pressure = PressureUnits.PoundsPerSquareInch;
            Units.Speed = SpeedUnits.MilesPerHour;
        }

        /// <summary>
        /// Custom date/time parsing function
        /// </summary>
        /// <param name="s">Date/time string in RFC822 Section 5 format</param>
        /// <returns>DateTime object containing specified date</returns>
        private DateTime ParseDateTime(string s)
        {
            // vars to hold the date time parameters
            int month, day, year, hour, minute;

            // break the date/time string into parts using a space as the delimiter
            string[] arr = s.Split(' ');
            // break the time part of the date/time string into hours and minutes
            string[] arrTime = arr[4].Split(':');

            // temporary DateTime object
            DateTime dt;

            // read the date/time from the array
            day = Convert.ToInt32(arr[1]);
            year = Convert.ToInt32(arr[3]);
            hour = Convert.ToInt32(arrTime[0]);
            minute = Convert.ToInt32(arrTime[1]);

            // Convert 12-hour to 24-hour time
            if (arr[5].ToLower() == "pm")
            {
                hour = hour + 12;
            }

            // Convert short month name to month number, 0 if error
            switch (arr[2])
            {
                case "Jan":
                    month = 1;
                    break;
                case "Feb":
                    month = 2;
                    break;
                case "Mar":
                    month = 3;
                    break;
                case "Apr":
                    month = 4;
                    break;
                case "May":
                    month = 5;
                    break;
                case "Jun":
                    month = 6;
                    break;
                case "Jul":
                    month = 7;
                    break;
                case "Aug":
                    month = 8;
                    break;
                case "Sep":
                    month = 9;
                    break;
                case "Oct":
                    month = 10;
                    break;
                case "Nov":
                    month = 11;
                    break;
                case "Dec":
                    month = 12;
                    break;
                default:
                    month = 0;
                    break;
            }

            // Create DateTime object
            dt = new DateTime(year, month, day, hour, minute, 0);
            // pass the DateTime object back to the caller
            return dt;
        }
    }

    /// <summary>
    /// Class to manage location information
    /// </summary>
    public class Location
    {
        /// <summary>
        /// City name
        /// </summary>
        public string City = "";
        /// <summary>
        /// State, territory, or region
        /// </summary>
        public string Region = "";
        /// <summary>
        /// Two-character country code
        /// </summary>
        public string Country = "";

        /// <summary>
        /// Location constructor
        /// </summary>
        /// <param name="city">City name</param>
        /// <param name="region">State, territory, or region</param>
        /// <param name="country">Two-character country code</param>
        public Location(string city, string region, string country)
        {
            City = city;
            Region = region;
            Country = country;
        }
    }

    /// <summary>
    /// Class to manage units for various aspects of the forecast
    /// </summary>
    public class Units
    {
        /// <summary>
        /// Temperature: degree units, f for Fahrenheit or c for Celsius
        /// </summary>
        public Weather.TemperatureUnits Temperature = Weather.TemperatureUnits.Null;
        /// <summary>
        /// Distance: units for distance, mi for miles or km for kilometers
        /// </summary>
        public Weather.DistanceUnits Distance = Weather.DistanceUnits.Null;
        /// <summary>
        /// Pressure: units of barometric pressure, in for pounds per square inch or mb for millibars
        /// </summary>
        public Weather.PressureUnits Pressure = Weather.PressureUnits.Null;
        /// <summary>
        /// Speed: units of speed, mph for miles per hour or kph for kilometers per hour
        /// </summary>
        public Weather.SpeedUnits Speed = Weather.SpeedUnits.Null;

        /// <summary>
        /// Units constructor
        /// </summary>
        /// <param name="temperature">Units for temperature</param>
        /// <param name="distance">Units for distance</param>
        /// <param name="pressure">Units for barometric pressure</param>
        /// <param name="speed">Unit for speed</param>
        public Units(Weather.TemperatureUnits temperature, Weather.DistanceUnits distance, Weather.PressureUnits pressure, Weather.SpeedUnits speed)
        {
            Temperature = temperature;
            Distance = distance;
            Pressure = pressure;
            Speed = speed;
        }

        /// <summary>
        /// Get the unit abbreviation string for temperature
        /// </summary>
        /// <returns>Unit abbreviation string</returns>
        public string GetTemperatureUnitAbbreviation()
        {
            switch (Temperature)
            {
                case Weather.TemperatureUnits.Celcius:
                    return "C";
                case Weather.TemperatureUnits.Fahrenheit:
                    return "F";
            }
            return null;
        }
        /// <summary>
        /// Get the unit abbreviation string for distance
        /// </summary>
        /// <returns>Unit abbreviation string</returns>
        public string GetDistanceUnitAbbreviation()
        {
            switch (Distance)
            {
                case Weather.DistanceUnits.Kilometeres:
                    return "KM";
                case Weather.DistanceUnits.Miles:
                    return "MI";
            }
            return null;
        }
        /// <summary>
        /// Get the unit abbreviation string for pressure
        /// </summary>
        /// <returns>Unit abbreviation string</returns>
        public string GetPressureUnitAbbreviation()
        {
            switch (Pressure)
            {
                case Weather.PressureUnits.Millibars:
                    return "MB";
                case Weather.PressureUnits.PoundsPerSquareInch:
                    return "PSI";
            }
            return null;
        }
        /// <summary>
        /// Get the unit abbreviation string for speed
        /// </summary>
        /// <returns>Unit abbreviation string</returns>
        public string GetSpeedUnitAbbreviation()
        {
            switch (Speed)
            {
                case Weather.SpeedUnits.KilometersPerHour:
                    return "KMH";
                case Weather.SpeedUnits.MilesPerHour:
                    return "MPH";
            }
            return null;
        }
    }

    /// <summary>
    /// Class to manage information about wind
    /// </summary>
    public class Wind
    {
        /// <summary>
        /// Wind chill in degrees
        /// </summary>
        public int Chill = -1;
        /// <summary>
        /// Wind direction, in degrees
        /// </summary>
        public int Direction = -1;
        /// <summary>
        /// Wind speed, in unit specified in Units class
        /// </summary>
        public int Speed = -1;

        /// <summary>
        /// Wind constructor
        /// </summary>
        /// <param name="chill">Wind chill in degrees</param>
        /// <param name="direction">Wind direction in degrees</param>
        /// <param name="speed">Wind speed in unit specified in Units class</param>
        public Wind(int chill, int direction, int speed)
        {
            Chill = chill;
            Direction = direction;
            Speed = speed;
        }
    }

    /// <summary>
    /// Class to manage information about current atmospheric conditions
    /// </summary>
    public class Atmosphere
    {
        /// <summary>
        /// Humidity, in percent
        /// </summary>
        public int Humidity = -1;
        /// <summary>
        /// Visibility, in the units specified by the Units class
        /// </summary>
        private decimal dVisibility = -1;
        public decimal Visibility
        {
            get
            {
                return dVisibility;
            }
            set
            {
                dVisibility = (decimal)value / (decimal)100;
            }
        }
        /// <summary>
        /// Barometric pressure, in the units specified by Units class
        /// </summary>
        public double Pressure = -1;
        /// <summary>
        /// State of the barometric pressure; steady, rising, falling
        /// </summary>
        public Weather.BarometricPressure Rising = 0;

        /// <summary>
        /// Atmosphere constructor
        /// </summary>
        /// <param name="humidity">Humidity, in percent</param>
        /// <param name="visibility">Visibility, in the units specified by the Units class</param>
        /// <param name="pressure">Barometric pressure, in the units specified by Units class</param>
        /// <param name="rising">State of the barometric pressure; steady, rising, falling</param>
        public Atmosphere(int humidity, int visibility, double pressure, Weather.BarometricPressure rising)
        {
            Humidity = humidity;
            Visibility = (decimal)visibility / (decimal)100;
            Pressure = pressure;
            Rising = rising;
        }
    }

    /// <summary>
    /// Class to manage information about current astronomical conditions
    /// </summary>
    public class Astronomy
    {
        /// <summary>
        /// Today's sunrise time
        /// </summary>
        public DateTime Sunrise = new DateTime();
        /// <summary>
        /// Today's sunset time
        /// </summary>
        public DateTime Sunset = new DateTime();

        /// <summary>
        /// Astronomy constructor
        /// </summary>
        /// <param name="sunrise">Today's sunrise time</param>
        /// <param name="sunset">Today's sunset time</param>
        public Astronomy(DateTime sunrise, DateTime sunset)
        {
            Sunrise = sunrise;
            Sunset = sunset;
        }
    }

    /// <summary>
    /// Class to manage information about the geographical location of the forecast
    /// </summary>
    public class Geography
    {
        /// <summary>
        /// The latitude of the location
        /// </summary>
        public decimal Lattitude = -1;
        /// <summary>
        /// The longitude of the location
        /// </summary>
        public decimal Longitude = -1;

        /// <summary>
        /// Geography constructor
        /// </summary>
        /// <param name="lattitude">The lattitude of the location</param>
        /// <param name="longitude">The longitude of the location</param>
        public Geography(decimal lattitude, decimal longitude)
        {
            Lattitude = lattitude;
            Longitude = longitude;
        }
    }

    /// <summary>
    /// Class to manage information about the current conditions
    /// </summary>
    public class Condition
    {
        /// <summary>
        /// A textual description of conditions
        /// </summary>
        public string Text = "";
        /// <summary>
        /// The condition code for this forecast
        /// </summary>
        public Weather.ConditionCodes Code = (Weather.ConditionCodes)3200;
        /// <summary>
        /// The current temperature, in the units specified by the Units class
        /// </summary>
        public int Temperature = -1;
        /// <summary>
        /// The current date and time for which this forecast applies
        /// </summary>
        public DateTime Date = new DateTime();

        /// <summary>
        /// Condition constructor
        /// </summary>
        /// <param name="text">A textual description of conditions</param>
        /// <param name="code">The condition code for this forecast</param>
        /// <param name="temperature">The current temperature, in the units specified by the Units class</param>
        /// <param name="date">The current date and time for which this forecast applies</param>
        public Condition(string text, Weather.ConditionCodes code, int temperature, DateTime date)
        {
            Text = text;
            Code = code;
            Temperature = temperature;
            Date = date;
        }

        public string GetPersianCode()
        {
            int WCode = (int)this.Code;
            string[] CodeNames = new string[50];
            CodeNames[0] = "گردباد";
            CodeNames[1] = "توفان گرمسيري";
            CodeNames[2] = "گردباد";
            CodeNames[3] = "توفان همراه با صاعقه شدید";
            CodeNames[4] = "توفان همراه با صاعقه";
            CodeNames[5] = "بارش برف و باران";
            CodeNames[6] = "بارش باران و تگرگ ريز";
            CodeNames[7] = "بارش برف و تگرگ ريز";
            CodeNames[8] = "نم نم باران یخزده";
            CodeNames[9] = "بارش نم نم باران";
            CodeNames[10] = "بارش باران يخ زده";
            CodeNames[11] = "رگبار";
            CodeNames[12] = "رگبار";
            CodeNames[13] = "بارش برف ناگهاني";
            CodeNames[14] = "بارش برف سبک";
            CodeNames[15] = "بارش برف سنگين";
            CodeNames[16] = "بارش برف";
            CodeNames[17] = "بارش تگرگ";
            CodeNames[18] = "بارش برف توام با بارن";
            CodeNames[19] = "گرد و خاک";
            CodeNames[20] = "مه آلود";
            CodeNames[21] = "غبار آلود";
            CodeNames[22] = "دود آلود";
            CodeNames[23] = "وزش باد شدید";
            CodeNames[24] = "وزش باد";
            CodeNames[25] = "سرد";
            CodeNames[26] = "ابري";
            CodeNames[27] = "شب نیمه ابری";
            CodeNames[28] = "روز نیمه ابری";
            CodeNames[29] = "شب تا حدودی ابری";
            CodeNames[30] = "روز تا حدودی ابری";
            CodeNames[31] = "شب صاف";
            CodeNames[32] = "روز آفتابی";
            CodeNames[33] = "شب بدون ابر";
            CodeNames[34] = "روز بدون ابر";
            CodeNames[35] = "بارش باران و تگرگ";
            CodeNames[36] = "داغ";
            CodeNames[37] = "توفان همراه با صاعقه";//Isolated
            CodeNames[38] = "توفان همراه با صاعقه پراکنده";
            CodeNames[39] = "توفان همراه با صاعقه پراکنده";
            CodeNames[40] = "رگبار پراکنده";
            CodeNames[41] = "بارش برف سنگین";
            CodeNames[42] = "بارش برف پراکنده";
            CodeNames[43] = "بارش برف سنگين";
            CodeNames[44] = "تاحدودی ابري";
            CodeNames[45] = "رعد و برق";
            CodeNames[46] = "بارش رگبار برف";
            CodeNames[47] = "رعد و برق پراکنده";//Isolated
            CodeNames[48] = "در دسترس نیست";
            return (WCode == 3200 ? CodeNames[48] : CodeNames[WCode]);
        }
    }

    /// <summary>
    /// Class to manage information about a forecasted day
    /// </summary>
    public class ForecastDay
    {
        /// <summary>
        /// The date to which this forecast applies
        /// </summary>
        public DateTime Date = new DateTime();
        /// <summary>
        /// The forecasted low temperature for this day
        /// </summary>
        public int Low = -1;
        /// <summary>
        /// The forecasted high temperature for this day
        /// </summary>
        public int High = -1;
        /// <summary>
        /// A textual description of conditions
        /// </summary>
        public string Text = "";
        /// <summary>
        /// The condition code for this forecast
        /// </summary>
        public Weather.ConditionCodes Code = (Weather.ConditionCodes)3200;

        /// <summary>
        /// Forecast day constructor
        /// </summary>
        /// <param name="date">The date to which this forecast applies</param>
        /// <param name="low">The forecasted low temperature for this day</param>
        /// <param name="high">The forecasted high temperature for this day</param>
        /// <param name="text">A textual description of conditions</param>
        /// <param name="code">The condition code for this forecast</param>
        public ForecastDay(DateTime date, int low, int high, string text, Weather.ConditionCodes code)
        {
            Date = date;
            Low = low;
            High = high;
            Text = text;
            Code = code;
        }
    }

    /// <summary>
    /// Class to contain a collection of ForecastDays
    /// </summary>
    public class ForecastDayCollection : CollectionBase
    {
        public virtual void Add(ForecastDay newForecastDay)
        {
            this.List.Add(newForecastDay);
        }
        public virtual ForecastDay this[int Index]
        {
            get
            {
                return (ForecastDay)this.List[Index];
            }
        }
    }

    /// <summary>
    /// Class to manage information about forecasted days
    /// </summary>
    public class Forecast
    {
        /// <summary>
        /// Collection of forecasted days
        /// </summary>
        public ForecastDayCollection Days = new ForecastDayCollection();

        /// <summary>
        /// Forecast constructor
        /// </summary>
        public Forecast()
        {

        }
    }
}