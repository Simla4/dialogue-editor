using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperUtility
{

    public static bool ValidateCheckEmptyString(Object thisObject, string fileName, string stringTocheck)
    {

        if(stringTocheck == "")
        {
            Debug.Log(fileName + " is empty and must contain a value in objetc" + thisObject.name.ToString());

            return true;
        }

        return false;
    }


    public static bool ValidationCheckEnurableValues(Object thisObject, string fieldName, IEnumerable enumarableObjectToCheck)
    {

        bool error = false;
        int count = 1;

        foreach(var item in enumarableObjectToCheck)
        {
            if(item == null)
            {
                Debug.Log(fieldName + " has null value in object" + thisObject.name.ToString());

                return true;
            }

            else
            {
                count++;
            }
        }

        if(count == 0)
        {
            Debug.Log(fieldName + " has null value in object" + thisObject.name.ToString());
            return error;
        }


        return error;
    }
}
