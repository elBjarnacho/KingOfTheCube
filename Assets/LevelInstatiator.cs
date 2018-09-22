﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum CubeFaces {
    firstFace = 1,
    secondFace = 2,
    thirdFace = 3,
    fourthFace = 4
}
public static class CubeLevel {
    public const float first = 0.5f;
    public const float second = 2.5f;
    public const float third = 4.5f;
    public const float fourth = 6.5f;
    public const float fifth = 8.5f;
    public const float sixth = 10.5f;
    public const float seventh = 12.5f;
    public const float eigth = 14.5f;
    public const float ninth = 16.5f;
    public const float tenth = 18.5f;
    public const float eleventh = 20.5f;
    public const float twelfth = 22.5f;
    public const float thirteenth = 24.5f;
    public const float fourtheenth = 26.5f;
    public const float fifteenth = 28.5f;
}

public static class CubeColumn {
    public const float first = 1;
    public const float second = 2;
    public const float third = 3;
    public const float fourth = 4;
    public const float fifth = 5;
    public const float sixth = 6;
    public const float seventh = 7;
    public const float eigth = 8;
    public const float ninth = 9;
    public const float tenth = 10;
    public const float eleventh = 11;
    public const float twelfth = 12;
    public const float thirteenth = 13;
    public const float fourtheenth = 14;
    public const float fifteenth = 15;
    public const float sixteenth = 16;
    public const float seventeenth = 17;
    public const float eighteenth = 18;
}

public class LevelInstatiator : MonoBehaviour{

    /********************\
     * Editor variables *
    \********************/ 

    public float scalingFactor = 0.1f;
    public Transform world;
    public Transform firstFaceCorner;
    public Transform secondFaceCorner;
    public Transform thirdFaceCorner;
    public Transform fourthFaceCorner;
    public Transform platformUnit;
    public Transform ladder;


    /*********************\
     * Private variables *
    \*********************/

    private float xBoundsMin = -15.5f;
    private float xBoundsMax = 15.5f;
    private float zBoundsMin = -15.5f;
    private float zBoundsMax = 15.5f;

    private Vector3 firstFaceVector;
    private Vector3 secondFaceVector;
    private Vector3 thirdFaceVector;
    private Vector3 fourthFaceVector;


    void Start() {

        Debug.Log("Configuring Level instantiator");

        xBoundsMin *= scalingFactor;
        xBoundsMax *= scalingFactor;
        zBoundsMin *= scalingFactor;
        zBoundsMax *= scalingFactor;

        firstFaceVector = new Vector3(xBoundsMax, 0f, zBoundsMin);
        secondFaceVector = new Vector3(xBoundsMax, 0f, zBoundsMax);
        thirdFaceVector = new Vector3(xBoundsMin, 0f, zBoundsMax);
        fourthFaceVector = new Vector3(xBoundsMin, 0f, zBoundsMin);
    }

    //private void instantiateFacePlatform(CubeFaces inFace, float atLevel, float atColumn, float distanceFromCube)

   

    public void buildLevel() {

        buildPlatform(CubeFaces.firstFace, CubeLevel.first, -1, 31);
        buildPlatform(CubeFaces.secondFace, CubeLevel.first, -1, 31);
        buildPlatform(CubeFaces.thirdFace, CubeLevel.first, -1, 31);
        buildPlatform(CubeFaces.fourthFace, CubeLevel.first, -1, 31);


        buildPlatform(CubeFaces.firstFace, CubeLevel.fourth, 20, 31);
        buildPlatform(CubeFaces.secondFace, CubeLevel.fourth, -1, 10);

        buildPlatform(CubeFaces.secondFace, CubeLevel.fourth, 15, 24);
        buildPlatform(CubeFaces.secondFace, CubeLevel.seventh, 18, 31);
        buildPlatform(CubeFaces.thirdFace, CubeLevel.seventh, -1, 10);

    }

    private void buildPlatform(CubeFaces inFace, float atLevel, int fromColumn, int toColumn)
    {
        for (int i = fromColumn; i <= toColumn; i++)
        {
            instantiateFacePlatform(inFace, atLevel, i, 0);
            instantiateFacePlatform(inFace, atLevel, i, 1);
        }
    }   
    
    private void instantiateFacePlatform(CubeFaces inFace, float atLevel, float atColumn, float distanceFromCube)
    {
        var copyVector = new Vector3(0, 0, 0);
        switch (inFace)
        {
            case CubeFaces.firstFace:
                copyVector = firstFaceVector;
                copyVector.x = xBoundsMax - (atColumn  * scalingFactor);
                copyVector.z = zBoundsMin - (distanceFromCube * scalingFactor);
                break;
            case CubeFaces.secondFace:
                copyVector = secondFaceVector;
                copyVector.z = zBoundsMax - (atColumn * scalingFactor);
                copyVector.x = xBoundsMax + (distanceFromCube * scalingFactor);
                break;
            case CubeFaces.thirdFace:
                copyVector = thirdFaceVector;
                copyVector.x = xBoundsMin + (atColumn * scalingFactor);
                copyVector.z = zBoundsMax + (distanceFromCube * scalingFactor);
                break;
            case CubeFaces.fourthFace:
                copyVector = fourthFaceVector;
                copyVector.z = zBoundsMin + (atColumn * scalingFactor);
                copyVector.x = xBoundsMin - (distanceFromCube * scalingFactor);
                break;
        }
        copyVector.y = atLevel * scalingFactor;
        Transform t = Instantiate(platformUnit, world, false);
        t.localPosition += copyVector;
    }    
}
