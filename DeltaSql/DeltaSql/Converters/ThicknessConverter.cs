﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DeltaSql.Converters
{
    public class ThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                Thickness thickness = (Thickness)value;
                Thickness adjusted = (Thickness)value;

                string param = parameter?.ToString();

                if (string.IsNullOrEmpty(param)) return thickness;

                string[] exploded = param.Split('|');

                foreach (string str in exploded)
                {
                    // example will be L:+3 or T:-4
                    string[] actionValue = str.Split(':');

                    // converter only supports addition and subtraction
                    if (!(actionValue[1][0] == '-' || actionValue[1][0] == '+'))
                    {
                        Debug.WriteLine($"ThicknessConverter does not support action '{actionValue[1][0]}'. It only supports addition (+) or subtraction (-).");

                        return thickness;
                    }

                    if (actionValue[0] == "L" || actionValue[0] == "l")
                    {
                        if (actionValue[1][0] != '+')
                        {
                            if (!double.TryParse(actionValue[1].Replace("+", ""), out double val))
                                return thickness;

                            adjusted.Left += val;
                        }
                        else
                        {
                            if (!double.TryParse(actionValue[1].Replace("-", ""), out double val))
                                return thickness;

                            adjusted.Left += val;
                        }
                    }
                    else if (actionValue[0] == "T" || actionValue[0] == "t")
                    {
                        if (actionValue[1][0] != '+')
                        {
                            if (!double.TryParse(actionValue[1].Replace("+", ""), out double val))
                                return thickness;

                            adjusted.Top += val;
                        }
                        else
                        {
                            if (!double.TryParse(actionValue[1].Replace("-", ""), out double val))
                                return thickness;

                            adjusted.Top += val;
                        }
                    }
                    else if (actionValue[0] == "R" || actionValue[0] == "r")
                    {
                        if (actionValue[1][0] != '+')
                        {
                            if (!double.TryParse(actionValue[1].Replace("+", ""), out double val))
                                return thickness;

                            adjusted.Right += val;
                        }
                        else
                        {
                            if (!double.TryParse(actionValue[1].Replace("-", ""), out double val))
                                return thickness;

                            adjusted.Right += val;
                        }
                    }
                    else if (actionValue[0] == "B" || actionValue[0] == "b")
                    {
                        if (actionValue[1][0] != '+')
                        {
                            if (!double.TryParse(actionValue[1].Replace("+", ""), out double val))
                                return thickness;

                            adjusted.Bottom += val;
                        }
                        else
                        {
                            if (!double.TryParse(actionValue[1].Replace("-", ""), out double val))
                                return thickness;

                            adjusted.Bottom += val;
                        }
                    }
                }

                return adjusted;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred attempting to convert the Thickness in the ThicknessConverter.{Environment.NewLine}{ex}");

                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}