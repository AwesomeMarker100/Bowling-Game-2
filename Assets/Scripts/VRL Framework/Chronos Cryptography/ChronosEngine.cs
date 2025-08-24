using System.Security.Cryptography;
using UnityEngine;

public class ChronosEngine
{

    private static char[] alphabet =
    {
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v',
        'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R',
        'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
    };

    public static int[] GenerateUniqueHashCodes(int numHashCodes, int stringLength)
    {
        int[] randomHashCodes = new int[numHashCodes];
        int i = 0;

        while (i < numHashCodes)
        {
            string randomString = GetRandomString(stringLength);
            int hashCode = randomString.GetHashCode();

            for (int j = 0; j < numHashCodes; j++)
            {
                if (hashCode == randomHashCodes[j])
                {
                    continue;
                }
            }

            randomHashCodes[i] = hashCode;

        }

        return randomHashCodes;
    }

    public static string GetRandomString(int stringLength)
    {
        string str = "";

        for(int i = 0; i < stringLength; i++)
        {
            int randIdx = Random.Range(0, alphabet.Length);
            char letterToAdd = alphabet[randIdx];
            str += letterToAdd;
        }

        return str;
    }


}
