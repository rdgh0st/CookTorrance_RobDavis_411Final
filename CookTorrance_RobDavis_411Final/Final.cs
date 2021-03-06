using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CookTorrance_RobDavis_411Final
{
    public class Final : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        SpriteFont font;
        Model activeModel, modelHeli, modelBunny, modelTeacup, modelSphere; // **** FBX file
        Effect effect;
        Texture2D texture;

        Matrix world = Matrix.Identity;
        Matrix view = Matrix.CreateLookAt(new Vector3(20, 0, 0), new Vector3(0, 0, 0), Vector3.UnitY);
        Matrix view_default = Matrix.CreateLookAt(new Vector3(20, 0, 0), new Vector3(0, 0, 0), Vector3.UnitY);
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 600f, 0.1f, 100f);
        Matrix lightView = Matrix.CreateLookAt(new Vector3(0, 0, 10), -Vector3.UnitZ, Vector3.UnitY);
        Matrix lightProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1f, 1f, 100f);

        Vector3 cameraPosition, cameraTarget, lightPosition;
        Vector4 ambientColor = new Vector4(0.25f, 0.25f, 0.25f, 1);
        float ambientIntensity = 0.5f;
        Vector4 diffuseColor = new Vector4(1, 1, 1, 1);
        float diffuseIntensity = 0.5f;
        Vector4 specularColor = new Vector4(1, 1, 1, 1);
        float roughness = 0.1f;
        Vector4 lightColor = new Vector4(1, 1, 1, 1);
        float F0 = 1;
        float angle, angle2, angleL, angleL2;
        float distance = 20f;
        MouseState preMouse;
        KeyboardState preKey;
        bool showHelp = true;

        public Final()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            effect = Content.Load<Effect>("CookTorrance");
            modelHeli = Content.Load<Model>("Helicopter");
            modelBunny = Content.Load<Model>("bunnyUV");
            modelTeacup = Content.Load<Model>("teapot");
            modelSphere = Content.Load<Model>("sphere");
            texture = Content.Load<Texture2D>("HelicopterTexture");
            font = Content.Load<SpriteFont>("font");
            activeModel = modelSphere;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Left)) angleL += 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) angleL -= 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) angleL2 += 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) angleL2 -= 0.02f;

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                angle -= (Mouse.GetState().X - preMouse.X) / 100f;
                angle2 += (Mouse.GetState().Y - preMouse.Y) / 100f;
            }
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                distance += (Mouse.GetState().X - preMouse.X) / 100f;
            }
            if (Mouse.GetState().MiddleButton == ButtonState.Pressed)
            {
                Vector3 ViewRight = Vector3.Transform(Vector3.UnitX,
                    Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                Vector3 ViewUp = Vector3.Transform(Vector3.UnitY,
                    Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                cameraTarget -= ViewRight * (Mouse.GetState().X - preMouse.X) / 10f;
                cameraTarget += ViewUp * (Mouse.GetState().Y - preMouse.Y) / 10f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.OemQuestion) && !preKey.IsKeyDown(Keys.OemQuestion))
            {
                showHelp = !showHelp;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.R) && !preKey.IsKeyDown(Keys.R))
            {
                if (!Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    roughness += 0.05f;
                }
                else
                {
                    roughness -= 0.05f;
                }
            }
            
            if (Keyboard.GetState().IsKeyDown(Keys.A) && !preKey.IsKeyDown(Keys.A))
            {
                if (!Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    ambientIntensity += 0.1f;
                }
                else
                {
                    ambientIntensity -= 0.1f;
                }
            }
            
            if (Keyboard.GetState().IsKeyDown(Keys.D) && !preKey.IsKeyDown(Keys.D))
            {
                if (!Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    diffuseIntensity += 0.1f;
                }
                else
                {
                    diffuseIntensity -= 0.1f;
                }
            }
            
            if (Keyboard.GetState().IsKeyDown(Keys.F1) && !preKey.IsKeyDown(Keys.F1))
            {
                if (!Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    lightColor.X += 0.1f;
                }
                else
                {
                    lightColor.X -= 0.1f;
                }
            }
            
            if (Keyboard.GetState().IsKeyDown(Keys.F2) && !preKey.IsKeyDown(Keys.F2))
            {
                if (!Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    lightColor.Y += 0.1f;
                }
                else
                {
                    lightColor.Y -= 0.1f;
                }
            }
            
            if (Keyboard.GetState().IsKeyDown(Keys.F3) && !preKey.IsKeyDown(Keys.F3))
            {
                if (!Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    lightColor.Z += 0.1f;
                }
                else
                {
                    lightColor.Z -= 0.1f;
                }
            }
            
            if (Keyboard.GetState().IsKeyDown(Keys.F) && !preKey.IsKeyDown(Keys.F))
            {
                if (!Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    F0 += 0.1f;
                }
                else
                {
                    F0 -= 0.1f;
                }
            }
            
            if (Keyboard.GetState().IsKeyDown(Keys.D1) && !preKey.IsKeyDown(Keys.D1))
            {
                activeModel = modelBunny;
            } else if (Keyboard.GetState().IsKeyDown(Keys.D2) && !preKey.IsKeyDown(Keys.D2))
            {
                activeModel = modelTeacup;
            } else if (Keyboard.GetState().IsKeyDown(Keys.D3) && !preKey.IsKeyDown(Keys.D3))
            {
                activeModel = modelSphere;
            } else if (Keyboard.GetState().IsKeyDown(Keys.D4) && !preKey.IsKeyDown(Keys.D4))
            {
                activeModel = modelHeli;
            }
            
            preKey = Keyboard.GetState();
            preMouse = Mouse.GetState();
            // Update Camera
            cameraPosition = Vector3.Transform(new Vector3(0, 0, distance),
                Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
            view = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Transform(Vector3.UnitY,
                Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle)));
            // Update Light
            lightPosition = Vector3.Transform(new Vector3(0, 0, 10), Matrix.CreateRotationX(angleL2) * Matrix.CreateRotationY(angleL));
            // Update LightMatrix
            lightView = Matrix.CreateLookAt(
                lightPosition,
                -Vector3.Normalize(lightPosition),
                Vector3.Up);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = new DepthStencilState();

            if (activeModel == modelHeli)
            {
                effect.CurrentTechnique = effect.Techniques[0];
            }
            else
            {
                effect.CurrentTechnique = effect.Techniques[1];
            }

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                foreach (ModelMesh mesh in activeModel.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        effect.Parameters["World"].SetValue(mesh.ParentBone.Transform);
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["Projection"].SetValue(projection);
                        Matrix worldInverseTransposeMatrix = Matrix.Transpose(
                            Matrix.Invert(mesh.ParentBone.Transform));
                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                        effect.Parameters["AmbientColor"].SetValue(ambientColor);
                        effect.Parameters["AmbientIntensity"].SetValue(ambientIntensity);

                        effect.Parameters["DiffuseColor"].SetValue(diffuseColor);
                        effect.Parameters["DiffuseIntensity"].SetValue(diffuseIntensity);

                        effect.Parameters["LightPosition"].SetValue(lightPosition);
                        effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                        effect.Parameters["SpecularColor"].SetValue(specularColor);
                        effect.Parameters["Roughness"].SetValue(roughness);
                        effect.Parameters["LightColor"].SetValue(lightColor);
                        effect.Parameters["F0"].SetValue(F0);
                        if (activeModel == modelHeli)
                        {
                            effect.Parameters["decalMap"].SetValue(texture);
                        }

                        pass.Apply(); // send the data to GPU
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;

                        GraphicsDevice.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList,
                            part.VertexOffset,
                            part.StartIndex,
                            part.PrimitiveCount);

                    }
                }
            }
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Roughness: " + roughness, Vector2.UnitX + Vector2.UnitY * 12, Color.White);
            spriteBatch.DrawString(font, "Ambient Color: " + ambientColor, Vector2.UnitX + Vector2.UnitY * 30, Color.White);
            spriteBatch.DrawString(font, "Ambient Intensity: " + ambientIntensity, Vector2.UnitX + Vector2.UnitY * 48, Color.White);
            spriteBatch.DrawString(font, "Diffuse Color: " + diffuseColor, Vector2.UnitX + Vector2.UnitY * 66, Color.White);
            spriteBatch.DrawString(font, "Diffuse Intensity: " + diffuseIntensity, Vector2.UnitX + Vector2.UnitY * 84, Color.White);
            spriteBatch.DrawString(font, "Light Position: " + lightPosition, Vector2.UnitX + Vector2.UnitY * 102, Color.White);
            spriteBatch.DrawString(font, "Light Color: " + lightColor, Vector2.UnitX + Vector2.UnitY * 120, Color.White);
            spriteBatch.DrawString(font, "Light Size: " + F0, Vector2.UnitX + Vector2.UnitY * 138, Color.White);
            if (activeModel == modelBunny)
            {
                spriteBatch.DrawString(font, "Current Model: Bunny",
                    Vector2.UnitX + Vector2.UnitY * 156, Color.White);
            }
            else if (activeModel == modelHeli)
            {
                spriteBatch.DrawString(font, "Current Model: Helicopter",
                    Vector2.UnitX + Vector2.UnitY * 156, Color.White);
            } else if (activeModel == modelTeacup)
            {
                spriteBatch.DrawString(font, "Current Model: Teacup",
                    Vector2.UnitX + Vector2.UnitY * 156, Color.White);
            } else if (activeModel == modelSphere)
            {
                spriteBatch.DrawString(font, "Current Model: Sphere",
                    Vector2.UnitX + Vector2.UnitY * 156, Color.White);
            }

            if (showHelp)
            {
                spriteBatch.DrawString(font, "Angle the camera: Drag Left Mouse", Vector2.UnitX + Vector2.UnitY * 176,
                    Color.White);
                spriteBatch.DrawString(font, "Zoom the camera: Drag Right Mouse", Vector2.UnitX + Vector2.UnitY * 194,
                    Color.White);
                spriteBatch.DrawString(font, "Pan the camera: Drag Middle Mouse", Vector2.UnitX + Vector2.UnitY * 212,
                    Color.White);
                spriteBatch.DrawString(font, "Rotate Light: Arrow Keys", Vector2.UnitX + Vector2.UnitY * 230,
                    Color.White);
                spriteBatch.DrawString(font, "Change Light Color: F2/F3/F4, Shift", Vector2.UnitX + Vector2.UnitY * 248,
                    Color.White);
                spriteBatch.DrawString(font, "Change Roughness: R/Shift and R", Vector2.UnitX + Vector2.UnitY * 266,
                    Color.White);
                spriteBatch.DrawString(font, "Change Model: 1/2/3/4", Vector2.UnitX + Vector2.UnitY * 284, Color.White);
                spriteBatch.DrawString(font, "Change Incident Size: F/Shift and F", Vector2.UnitX + Vector2.UnitY * 302,
                    Color.White);
                spriteBatch.DrawString(font, "Help: ?", Vector2.UnitX + Vector2.UnitY * 320, Color.White);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}