using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGenerator : MonoBehaviour
{

    [Header("Default Variables")]
    public int mainHexagonRingInt = 4;
    public GameObject hexagonPrefab;
    public List<GameObject> wholePartList;
    public bool hexaBoolForRotations;


    [Header("Hexagon Store")]
    public List<GameObject> storeHexagonList; // Svi hexagoni
    public List<int> borderIntList; // od kog prstena do kog 

    public List<GameObject> ringList;


    [Header("Offsets")]
    public float xOffset = 0.66f;
    public float yOffset = 0.76f;
    public float ySecondOffset = 0.38f;

    [Header("Hexagon Dimension")]
    public float R;
    public float H;
    public float S;
    public List<Vector2> primaryHexaPositions;

    [Header("Shape Variables")]
    public List<int> gapsAndSaves;
    public List<int> previusGapPositions;
    private int numberOfGaps;


    [Header("Parts")]
    public List<GameObject> partList;
    public List<Color> colorList;
    public List<int> partsPerGap;
    private int partsInCentar;




    [Header("Center of gravity")]
    public List<Vector2> centerFromParts;


    [Header("Levels")]
    public int level = 5;



    public void mathCalculation()
    {

        H = (Mathf.Sqrt(3) * R);

        S = (3 * R) / 2;

        // Zavisi rotacija 1 2 3 4 ... umesto 0 6 5 4...

        if (hexaBoolForRotations == true)
        {
            primaryHexaPositions[0] = new Vector2(H, S);

            primaryHexaPositions[5] = new Vector2(H, 2 * R - S);

            primaryHexaPositions[4] = new Vector2(H / 2, 0);

            primaryHexaPositions[3] = new Vector2(0, 2 * R - S);

            primaryHexaPositions[2] = new Vector2(0, S);

            primaryHexaPositions[1] = new Vector2(H / 2, 2 * R);
        }
        else
        {
            primaryHexaPositions[0] = new Vector2(H, S);

            primaryHexaPositions[1] = new Vector2(H, 2 * R - S);

            primaryHexaPositions[2] = new Vector2(H / 2, 0);

            primaryHexaPositions[3] = new Vector2(0, 2 * R - S);

            primaryHexaPositions[4] = new Vector2(0, S);

            primaryHexaPositions[5] = new Vector2(H / 2, 2 * R);
        }

        

        for (int i = 0; i < 6; i++)
        {
            primaryHexaPositions[i] = new Vector2(primaryHexaPositions[i].x - H / 2, primaryHexaPositions[i].y - S / 2); // (0 , 0)
        }

    }


    private void Start()
    {

        setListSize<Vector2>(primaryHexaPositions, 6);
       
        findBorder();

        mathCalculation();

        startGenerate();


        ringUpdate();


        //New Shape
        newShape();


    }


    private void startGenerate()
    {

        setListSize<GameObject>(wholePartList, 6);
        setListSize<GameObject>(storeHexagonList, borderIntList[mainHexagonRingInt]);

        for (int p = 0; p < 6; p++)
        {

            for (int x = 0; x < mainHexagonRingInt; x++)
            {

                for (int y = 0; y < x + 1; y++)
                {

                    Vector2 spawnPoint = new Vector2(x * xOffset, y * yOffset);


                    if (x % 2 != 0) //Neparni brojevi
                    {
                        spawnPoint = new Vector2(spawnPoint.x, spawnPoint.y - (x - 1) * ySecondOffset);
                        spawnPoint = new Vector2(spawnPoint.x, spawnPoint.y - ySecondOffset);
                    }
                    else
                    {
                        spawnPoint = new Vector2(spawnPoint.x, spawnPoint.y - x * ySecondOffset);
                    }

                    GameObject spawnedHexagon = Instantiate(hexagonPrefab, spawnPoint, Quaternion.identity);

                    spawnedHexagon.name = "hex_" + x + "_" + y;

                    if (wholePartList[p] != null)
                    {

                        spawnedHexagon.transform.SetParent(wholePartList[p].transform);

                    }
                    else
                    {
                        wholePartList[p] = spawnedHexagon;
                        spawnedHexagon.name = "hex_"  + (p + 1);
                    }


                    //Store
                    storeHexagonList[p * (x + 1) + borderIntList[x] + y] = spawnedHexagon;

                    //storeHexagonList[p + borderIntList[x] + 6 * y] = spawnedHexagon; //Old


                }
            }

            wholePartList[p].transform.Rotate(0, 0, p * 60);

            wholePartList[p].transform.position = primaryHexaPositions[p];


        }



    }


    public void newRestart() //Later Delite
    {

        mathCalculation();

        for (int p = 0; p < 6; p++)
        {
            wholePartList[p].transform.position = primaryHexaPositions[p];
        }

    }

    private void setListSize<listVariable>(List<listVariable> newList , int listSize)
    {
        
        newList.Clear();

        for (int i = 0; i < listSize; i++)
        {
            newList.Add(default(listVariable));
        }

    }

    private void findBorder()
    {

        setListSize<int>(borderIntList, mainHexagonRingInt + 1);

        int someInt = 0;

        for (int i = 0; i < mainHexagonRingInt + 1; i++)
        {
            someInt = i * 6;
            borderIntList[i] += someInt;
        }

        for (int i = 1; i < borderIntList.Count; i++)
        {
            borderIntList[i] = borderIntList[i] + borderIntList[i - 1];
        }

    }

    private void ringUpdate()
    {

        for (int i = 0; i < ringList.Count; i++) //Delite old rings
        {
            Destroy(ringList[i].gameObject);
        }

        setListSize<GameObject>(ringList, 0);

        for (int i = 0; i < mainHexagonRingInt; i++) //Generate new empty rings
        {

            GameObject newEmptyGameObject = new GameObject();

            newEmptyGameObject.name = "Ring " + (i + 1);

            ringList.Add(newEmptyGameObject);

        }

        for (int i = 0; i < mainHexagonRingInt; i++) // Fill rings
        {

            for (int b = borderIntList[i]; b < borderIntList[i + 1]; b++) //Krece od 0 ide do 6 itd ... 6 do 18
            {

                storeHexagonList[b].gameObject.transform.SetParent(ringList[i].transform); //Pakuje u prsten

            }

        }

    }

    public void newGenerate()
    {

        for (int i = 0; i < partList.Count; i++)
        {
            Destroy(partList[i]);
        }

        setListSize<GameObject>(partList, 0);

        startGenerate();
        ringUpdate();
        newShape();

    }


    public void newShape()
    {

        int numberOfHexaInRing = 6 * mainHexagonRingInt; // Hexa number in this max ring
        int gapStart = Random.Range(borderIntList[mainHexagonRingInt - 1], borderIntList[mainHexagonRingInt]);

        numberOfGaps = Random.Range(mainHexagonRingInt - 2, mainHexagonRingInt + 1);

        setListSize<int>(gapsAndSaves, numberOfGaps * 2);
        setListSize<int>(previusGapPositions, numberOfGaps * 2);

        #region Generate Default Numbers
        for (int i = 0; i < numberOfGaps; i++) //Generate minimum number of hexa per pause and gap
        {

            int hexaInThisGap = Random.Range(1, mainHexagonRingInt + 1);

            int hexaInThisSave = 2;

            numberOfHexaInRing -= (hexaInThisGap + hexaInThisSave);

            gapsAndSaves[i] = hexaInThisGap;
            gapsAndSaves[i + numberOfGaps] = hexaInThisSave;

        }
        #endregion

        #region Generate Rest of Numbers
        for (int i = 0; i < numberOfHexaInRing; i++) //Generate rest of hexa randomly in pause or gap
        {

            int randomGapOrSave = Random.Range(0, gapsAndSaves.Count);

            gapsAndSaves[randomGapOrSave]++;

        }
        #endregion

        generateNumberOfParts();

        #region Fix First Pause
        //Fix first pause;
        //setListSize<GameObject>(partList, numberOfGaps);

        int saveFirstPouse = gapsAndSaves[numberOfGaps];

        int randomFirstPouse = Random.Range(0, (gapsAndSaves[numberOfGaps] + 1)); //Possible delite, but fix in second posue part
        int firstPouseOffset = gapsAndSaves[numberOfGaps] - randomFirstPouse; 
        gapsAndSaves[numberOfGaps] = randomFirstPouse; // da ne bi pocinjalo uvek na istom mestu prva pazua ce mozda biti i iza


        GameObject saveFirstPart = null;
        GameObject saveSecondPart = null;

        if (partsPerGap[0] == 1)
        {

            int whereToCreateFirstPart = gapsAndSaves[numberOfGaps] - (saveFirstPouse - (saveFirstPouse / 2));

            if (whereToCreateFirstPart < 0) //Infinity loop, ako je - 3 bice childCount - 3
            {
                whereToCreateFirstPart = ringList[mainHexagonRingInt - 1].transform.childCount + whereToCreateFirstPart;
            }

            saveFirstPart = ringList[mainHexagonRingInt - 1].transform.GetChild(whereToCreateFirstPart).gameObject;

            //ringList[mainHexagonRingInt - 1].transform.GetChild(whereToCreateFirstPart).gameObject.GetComponent<SpriteRenderer>().color = Color.red; // De color

        }
        else if (partsPerGap[0] == 2)
        {
            //First Part
            int whereToCreateFirstPart = 0 - firstPouseOffset;

            if (whereToCreateFirstPart < 0) 
            {
                whereToCreateFirstPart = ringList[mainHexagonRingInt - 1].transform.childCount + whereToCreateFirstPart;
            }

            saveFirstPart = ringList[mainHexagonRingInt - 1].transform.GetChild(whereToCreateFirstPart).gameObject;

            //Second Part
            int whereToCreateSecondPart = gapsAndSaves[numberOfGaps] - 1;

            if (whereToCreateSecondPart < 0)
            {
                whereToCreateSecondPart = ringList[mainHexagonRingInt - 1].transform.childCount + whereToCreateSecondPart;
            }

            saveSecondPart = ringList[mainHexagonRingInt - 1].transform.GetChild(whereToCreateSecondPart).gameObject;

            //ringList[mainHexagonRingInt - 1].transform.GetChild(whereToCreateFirstPart).gameObject.GetComponent<SpriteRenderer>().color = Color.red; // De color
            //ringList[mainHexagonRingInt - 1].transform.GetChild(whereToCreateSecondPart).gameObject.GetComponent<SpriteRenderer>().color = Color.red; // De color


        }


        #endregion

        #region Main Generate - Last Ring

        int currentInt = 0;

        for (int q = 0; q < numberOfGaps; q++)
        {

            currentInt += gapsAndSaves[numberOfGaps + q]; // pauziram

            int maxInt = currentInt + gapsAndSaves[q];

            maxInt = Mathf.Clamp(maxInt, 0, mainHexagonRingInt * 6);

            if (partsPerGap[q] == 1)
            {
                int whereToCreateNewPart = currentInt - (gapsAndSaves[q + numberOfGaps] - (gapsAndSaves[q + numberOfGaps] / 2));

                #region Debug Centar of Pouse
                //Debug.Log("Trenutan int: " + currentInt + " ukupno ima u prstenu: " + ringList[mainHexagonRingInt - 1].transform.childCount);

                //ringList[mainHexagonRingInt - 1].transform.GetChild(whereToCreateNewPart).gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
                #endregion

                partList.Add(ringList[mainHexagonRingInt - 1].transform.GetChild(whereToCreateNewPart).gameObject);
            }
            else if(partsPerGap[q] == 2)
            {
                int whereToCreateNewPart = currentInt - (gapsAndSaves[q + numberOfGaps]);

                #region Debug Centar of Pouse
                //Debug.Log("Trenutan int: " + currentInt + " ukupno ima u prstenu: " + ringList[mainHexagonRingInt - 1].transform.childCount);

                //ringList[mainHexagonRingInt - 1].transform.GetChild(whereToCreateNewPart).gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
                #endregion

                partList.Add(ringList[mainHexagonRingInt - 1].transform.GetChild(whereToCreateNewPart).gameObject);


                //Secon Part
                whereToCreateNewPart = currentInt - 1;

                if (whereToCreateNewPart < 0)
                {
                    whereToCreateNewPart = 0;
                }

                #region Debug Centar of Pouse
                //Debug.Log("Trenutan int: " + currentInt + " ukupno ima u prstenu: " + ringList[mainHexagonRingInt - 1].transform.childCount);

                //ringList[mainHexagonRingInt - 1].transform.GetChild(whereToCreateNewPart).gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
                #endregion

                partList.Add(ringList[mainHexagonRingInt - 1].transform.GetChild(whereToCreateNewPart).gameObject);

            }

            
            // od pauze do gepa
            for (int i = currentInt; i < maxInt; i++)
            {

                #region De Bug Magic Of Color
                // ... Color Debug ...
                //ringList[mainHexagonRingInt - 1].transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
                #endregion

                storeHexagonList.Remove(ringList[mainHexagonRingInt - 1].transform.GetChild(i).gameObject);
                Destroy(ringList[mainHexagonRingInt - 1].transform.GetChild(i).gameObject);

            }

            previusGapPositions[q] = currentInt; // belezim gde sam poceo nakon prethodne pauze

            currentInt += gapsAndSaves[q]; // dodajem gep

            previusGapPositions[q + numberOfGaps] = currentInt; // belezim gde sam stao nakon dodatog gapa

        }

        if (partsPerGap[0] == 1)
        {
            partList[0] = saveFirstPart; //Ubacuje centar prve fixovane pauze
        }
        else if (partsPerGap[0] == 2)
        {
            partList[0] = saveFirstPart;
            partList[1] = saveSecondPart;
        }

        

#endregion

        #region Create Final Shape

        int gapsInNextRings = Random.Range(1, mainHexagonRingInt - 2);

        for (int i = 0; i < gapsInNextRings; i++)
        {

            nextRing(mainHexagonRingInt - (2 + i));

        }
        #endregion


        addPartsInCentar();


        makeParts();

    }


    public void nextRing(int previusRing)
    {

        int hexaInThisRing = (previusRing + 1) * 6;

        for (int i = 0; i < numberOfGaps; i++)
        {

            int startPosition = previusGapPositions[i] - 1 - ((previusGapPositions[i] - 1) / (previusRing + 2));

            int endPosition = previusGapPositions[i + numberOfGaps] - 1 - ((previusGapPositions[i + numberOfGaps] - 1) / (previusRing + 2));


            int randomHexas = Random.Range(0, gapsAndSaves[i]);

            startPosition = Random.Range(startPosition, endPosition - randomHexas + 1);

            //Save
            gapsAndSaves[i] = randomHexas;
            previusGapPositions[i] = startPosition;
            previusGapPositions[i + numberOfGaps] = endPosition;

            for (int b = Mathf.Clamp(startPosition , 0 , hexaInThisRing) ; b < Mathf.Clamp(startPosition + randomHexas, 0, hexaInThisRing) ; b++)
            {

                // ... Color Debug ...
                //ringList[previusRing].transform.GetChild(b).gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;


                storeHexagonList.Remove(ringList[previusRing].transform.GetChild(b).gameObject);
                Destroy(ringList[previusRing].transform.GetChild(b).gameObject);

            }

        }

    }



    private void generateNumberOfParts()
    {


        // ... NOW MAKE PART ///



        // Set number of parts based on skill level

        int delilac = (int)(9 + Mathf.Round((float)level / 10));

        int numberOfParts = (int)(((float)level / delilac) * 12); // 12 - max parts


        int periferParts = Random.Range(numberOfParts - 5, numberOfGaps * 2 + 1);

        partsInCentar = numberOfParts - periferParts;

        #region Generate Number Of Perifer Parts

        setListSize<int>(partsPerGap, numberOfGaps); //Reuse existing list
        //setListSize<GameObject>(partList, numberOfGaps);

        int realNumberOfParts = 0;
        int prtsPerTgisGap;

        for (int i = 0; i < numberOfGaps; i++) //Generate incorect number of parts per gap
        {

            prtsPerTgisGap = Random.Range(0, 3); // 0 1 2

            realNumberOfParts += prtsPerTgisGap;
            partsPerGap[i] = prtsPerTgisGap;

        }

        int numberToAdjust = periferParts - realNumberOfParts;

        int positiveOrNegInt;
        int minimumInt;

        if (numberToAdjust < 0)
        {
            positiveOrNegInt = -1;
            minimumInt = 0;
        }
        else
        {
            positiveOrNegInt = 1;
            minimumInt = 2;
        }


        for (int i = 0; i < Mathf.Abs(numberToAdjust); i++)  //Generate corect number of parts
        {

            int randomGap = Random.Range(0, numberOfGaps);

            if (partsPerGap[randomGap] == minimumInt) // Dont make - 1 or to big value
            {
                for (int b = 0; b < numberOfGaps; b++)
                {
                    if (partsPerGap[b] != minimumInt)
                    {

                        partsPerGap[b] += positiveOrNegInt;
                        break;

                    }
                }
            }
            else
            {
                partsPerGap[randomGap] += positiveOrNegInt;
            }

        }

        #endregion






        // ... END ... ///


        return;

        #region Generate Perifer Parts
        for (int i = 0; i < numberOfGaps; i++) // Generate already existing parts
        {

            GameObject newPart = partList[i];

            storeHexagonList.Remove(newPart);

            newPart.GetComponent<SpriteRenderer>().color = colorList[i];

            // Center Of Gravity
            centerFromParts[i] = newPart.transform.position;

        }

        for (int i = 0; i < 6; i++)
        {

            GameObject newPart = ringList[0].transform.GetChild(i).gameObject;

            storeHexagonList.Remove(newPart);

            newPart.GetComponent<SpriteRenderer>().color = colorList[numberOfGaps + i];

            partList.Add(newPart);

            // Center Of Gravity
            centerFromParts[numberOfGaps + i] = newPart.transform.position;

        }

        #endregion

        #region Fill Parts With Hexagons
        for (int i = 0; i < storeHexagonList.Count; i++) // Fill Parts
        {


            float distanceBetweenHexas = Vector2.Distance(partList[0].transform.position, storeHexagonList[i].transform.position);

            float closerDistance = distanceBetweenHexas;

            GameObject closerPart = partList[0];

            int closerIntPart = 0;

            for (int p = 1; p < partList.Count; p++)
            {

                distanceBetweenHexas = Vector2.Distance(partList[p].transform.position, storeHexagonList[i].transform.position);

                if (distanceBetweenHexas < closerDistance)
                {

                    closerDistance = distanceBetweenHexas;

                    closerPart = closerPart = partList[p];

                    closerIntPart = p;

                }


            }

            storeHexagonList[i].gameObject.transform.SetParent(closerPart.transform);

            storeHexagonList[i].GetComponent<SpriteRenderer>().color = closerPart.GetComponent<SpriteRenderer>().color; // Color Parts

            // Center Of Gravity
            centerFromParts[closerIntPart] += (Vector2)storeHexagonList[i].transform.position;


        }
        #endregion

    }

    private void addPartsInCentar()
    {

        int newPartPosition;

        for (int i = 0; i < partsInCentar; i++)
        {

            newPartPosition = 6 / partsInCentar;

            GameObject newPart = ringList[0].transform.GetChild(i * newPartPosition).gameObject;

            storeHexagonList.Remove(newPart);

            partList.Add(newPart);

            // Center Of Gravity
            //centerFromParts[numberOfGaps + i] = newPart.transform.position;

        }

        //Color Parts
        for (int i = 0; i < partList.Count; i++)
        {
            partList[i].gameObject.GetComponent<SpriteRenderer>().color = colorList[i];
        }

    }


    private void makeParts()
    {

        #region Fill Parts With Hexagons
        for (int i = 0; i < storeHexagonList.Count; i++) // Fill Parts
        {


            float distanceBetweenHexas = Vector2.Distance(partList[0].transform.position, storeHexagonList[i].transform.position);

            float closerDistance = distanceBetweenHexas;

            GameObject closerPart = partList[0];

            int closerIntPart = 0;

            for (int p = 1; p < partList.Count; p++)
            {

                distanceBetweenHexas = Vector2.Distance(partList[p].transform.position, storeHexagonList[i].transform.position);

                if (distanceBetweenHexas < closerDistance)
                {

                    closerDistance = distanceBetweenHexas;

                    closerPart = closerPart = partList[p];

                    closerIntPart = p;

                }

            }

            storeHexagonList[i].gameObject.transform.SetParent(closerPart.transform);

            storeHexagonList[i].GetComponent<SpriteRenderer>().color = closerPart.GetComponent<SpriteRenderer>().color; // Color Parts

            // Center Of Gravity
            //centerFromParts[closerIntPart] += (Vector2)storeHexagonList[i].transform.position;


        }
        #endregion

    }


    void OnDrawGizmos()
    {

        for (int i = 0; i < centerFromParts.Count; i++)
        {

            Gizmos.color = colorList[i];

            Vector2 whereToDraw = centerFromParts[i] / partList[i].transform.childCount;

            Gizmos.DrawSphere(whereToDraw, 0.3f);

        }

       

    }





}
