using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorHarmonyDemo : MonoBehaviour
{
    public Color sourceColor;
    public Image[] sourceColorImages;
    public Image complimentary;
    public Image splitComp1;
    public Image splitComp2;
    public Image triadic1;
    public Image triadic2;
    public Image tetradic1;
    public Image tetradic2;
    public Image tetradic3;
    public Image analogous1;
    public Image analogous2;
    public Image mono1;
    public Image mono2;

    IEnumerator Start()
    {
        if (sourceColor.a != 0f)
        {
            yield break;
        }
        else
        {
            while (true)
            {
                sourceColor = Wrj.FlatUIPalette.Colors.GetRandom();
                yield return new WaitForSeconds(2f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var item in sourceColorImages)
        {
            item.color = sourceColor;
        }
        complimentary.color = Wrj.ColorHarmony.Complementary(sourceColor);
        var split = Wrj.ColorHarmony.SplitComplementary(sourceColor);
        splitComp1.color = split[0];
        splitComp2.color = split[2];
        var triad = Wrj.ColorHarmony.Triadic(sourceColor);
        triadic1.color = triad[0];
        triadic2.color = triad[2];
        var tet = Wrj.ColorHarmony.Tetradic(sourceColor);
        tetradic1.color = tet[1];
        tetradic2.color = tet[2];
        tetradic3.color = tet[3];
        var anal = Wrj.ColorHarmony.Analogous(sourceColor);
        analogous1.color = anal[0];
        analogous2.color = anal[2];
        var mono = Wrj.ColorHarmony.Monochromatic(sourceColor);
        mono1.color = mono[0];
        mono2.color = mono[2];
    }
}
