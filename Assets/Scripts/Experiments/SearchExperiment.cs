using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchExperiment : Experiment
{

    List<List<List<int>>> shapes = new List<List<List<int>>>();
    List<Color> colors = new List<Color>();
    int mode;
    int border = 0;
    Color white = Color.white;


    public SearchExperiment(int _mode) {

        // correctAnswer - 0 if generated image doesn't contain target shape (left button)
        // correctAnswer - 1 if generated image contains target shape with target color (right button)
        // target shape is the first element in shapes list (similarily for target color)

        mode = _mode;

        // No special button feature
        specialButtonFeatureCode = 0;
        
        Start();

    }
    

    void Start()
    {

        List<List<int>> vertical = new List<List<int>>(){
            new List<int>(){0, 1, 0, 0},
            new List<int>(){0, 1, 0, 0},
            new List<int>(){0, 1, 0, 0},
            new List<int>(){0, 0, 0, 0},
        };

        List<List<int>> horizontal1 = new List<List<int>>(){
            new List<int>(){1, 1, 1, 0},
            new List<int>(){0, 0, 0, 0},
            new List<int>(){0, 0, 0, 0},
            new List<int>(){0, 0, 0, 0},
        };

        List<List<int>> horizontal2 = new List<List<int>>(){
            new List<int>(){0, 0, 0, 0},
            new List<int>(){1, 1, 1, 0},
            new List<int>(){0, 0, 0, 0},
            new List<int>(){0, 0, 0, 0},
        };

        List<List<int>> horizontal3 = new List<List<int>>(){
            new List<int>(){0, 0, 0, 0},
            new List<int>(){0, 0, 0, 0},
            new List<int>(){1, 1, 1, 0},
            new List<int>(){0, 0, 0, 0},
        };


        shapes.Add(vertical);
        shapes.Add(horizontal1);
        shapes.Add(horizontal2);
        shapes.Add(horizontal3);


        Color magenta = new Color(255, 0, 191);
        Color yellow = new Color(255, 191, 0);

        colors.Add(magenta);
        colors.Add(yellow);


    }

    public override Texture2D GetNextTexture()
    {
        // 0 - color is always same, shape is not target shape
        // 1 - shape is always same, color is not target color
        // 2 - both 0 and 1
        // 3 - random: either 0, 1 or 2

        // If random mode, pick other mode here
        int newMode;
        if (mode == 3) {
            newMode = Random.Range(0, 3);
        } else {
            newMode = mode;
        }


        Texture2D texture = new Texture2D(32 + 2 * border, 32 + 2 * border, TextureFormat.ARGB32, false);

        // Iterate through the 8 * 8 grid, each grid tile is 4*4 pixels, also takes border into account
        for (int i = border; i < 32 + border; i += 4) {
            for (int j = border; j < 32 + border; j += 4) {
                
                // 65% chance that tile contains a shape
                if (Random.Range(0, 100) < 65){
                    
                    // The shape that will be used, depending on mode
                    int randomShapeInt = 0;
                    List<List<int>> randomShape;
                    if (newMode == 1) {
                        randomShape = shapes[0];
                    } else if (newMode == 0) {
                        randomShape = shapes[Random.Range(1, shapes.Count)];
                    } else {
                        randomShapeInt = Random.Range(0, shapes.Count);
                        randomShape = shapes[randomShapeInt];
                    }

                    // The color that will be used for the shape, depending on mode
                    Color randomColor;
                    if (newMode == 0) {
                        randomColor = colors[0];
                    } else if (newMode == 1){
                        randomColor = colors[Random.Range(1, colors.Count)];
                    } else if (randomShapeInt == 0) {
                        // If shape is same as target shape the color has to be different
                        randomColor = colors[Random.Range(1, colors.Count)];
                    } else {
                        randomColor = colors[Random.Range(0, colors.Count)];
                    }

                    // Draw the pixels of the shape onto the texture (using previously selected color)
                    for (int x = 0; x < randomShape.Count; x++) {
                        for (int y = 0; y < randomShape[0].Count; y++) {
                            if (randomShape[x][y] == 1){
                                texture.SetPixel(y + j, x + i, randomColor);
                            } else {
                                texture.SetPixel(y + j, x + i, white);
                            }
                        }
                    }

                // If the tile doesn't contain a shape it needs to be made white (default color is not white)
                } else {
                    for (int x = 0; x < shapes[0].Count; x++) {
                        for (int y = 0; y < shapes[0][0].Count; y++) {
                            texture.SetPixel(y + j, x + i, white);
                        }
                    }


                }
            }
        }

        // 50% chance that the target shape with target color will be added
        if (Random.Range(0,2) == 0){
            // Set whether it exists to public variable so that it could be retrieved as needed
            correctAnswer = 1;
            // Pick a random grid tile (starting coordinates)
            int targetX = border + 4 * Random.Range(0, 9);
            int targetY = border + 4 * Random.Range(0, 9);
            // Select target shape and color (target shape and color are the first elements in their lists)
            List<List<int>> targetShape = shapes[0];
            Color targetColor = colors[0];

            // Draw the pixels of the target shape onto the texture, overwriting what was there previously
            for (int x = 0; x < targetShape.Count; x++){
                for (int y = 0; y < targetShape[0].Count; y ++) {
                    if (targetShape[x][y] == 1) {
                        texture.SetPixel(y + targetY, x + targetX, targetColor);
                    } else {
                        texture.SetPixel(y + targetY, x + targetX, white);
                    }
                }
            }
        } else {
            // Set whether it exists to public variable so that it could be retrieved as needed
            correctAnswer = 0;
        }

        texture.Apply();
        // Disable filter so the texture won't be blurred
        texture.filterMode = FilterMode.Point;
        // Set wrap mode to clamp to prevent colors bleeding over to opposite edges
        texture.wrapMode = TextureWrapMode.Clamp;

        return texture;

    }

}
