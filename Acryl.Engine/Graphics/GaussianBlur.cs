//-----------------------------------------------------------------------------
// Copyright (c) 2008-2011 dhpoware. All Rights Reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//-----------------------------------------------------------------------------

using System;
using Acryl.Engine.Stores;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Engine.Graphics
{
    /// <summary>
    /// A Gaussian blur filter kernel class. A Gaussian blur filter kernel is
    /// perfectly symmetrical and linearly separable. This means we can split
    /// the full 2D filter kernel matrix into two smaller horizontal and
    /// vertical 1D filter kernel matrices and then perform the Gaussian blur
    /// in two passes. Contrary to what you might think performing the Gaussian
    /// blur in this way is actually faster than performing the Gaussian blur
    /// in a single pass using the full 2D filter kernel matrix.
    /// <para>
    /// The GaussianBlur class is intended to be used in conjunction with an
    /// HLSL Gaussian blur shader. The following code snippet shows a typical
    /// Effect file implementation of a Gaussian blur.
    /// <code>
    /// #define RADIUS  7
    /// #define KERNEL_SIZE (RADIUS * 2 + 1)
    ///
    /// float weights[KERNEL_SIZE];
    /// float2 offsets[KERNEL_SIZE];
    ///
    /// texture colorMapTexture;
    ///
    /// sampler2D colorMap = sampler_state
    /// {
    ///     Texture = <![CDATA[<colorMapTexture>;]]>
    ///     MipFilter = Linear;
    ///     MinFilter = Linear;
    ///     MagFilter = Linear;
    /// };
    ///
    /// float4 PS_GaussianBlur(float2 texCoord : TEXCOORD) : COLOR0
    /// {
    ///     float4 color = float4(0.0f, 0.0f, 0.0f, 0.0f);
    ///
    ///     <![CDATA[for (int i = 0; i < KERNEL_SIZE; ++i)]]>
    ///         color += tex2D(colorMap, texCoord + offsets[i]) * weights[i];
    ///
    ///     return color;
    /// }
    ///
    /// technique GaussianBlur
    /// {
    ///     pass
    ///     {
    ///         PixelShader = compile ps_2_0 PS_GaussianBlur();
    ///     }
    /// }
    /// </code>
    /// The RADIUS constant in the effect file must match the radius value in
    /// the GaussianBlur class. The effect file's weights global variable
    /// corresponds to the GaussianBlur class' kernel field. The effect file's
    /// offsets global variable corresponds to the GaussianBlur class'
    /// offsetsHoriz and offsetsVert fields.
    /// </para>
    /// </summary>
    public class GaussianBlur : DependencyContainer
    {
        private readonly SpriteBatch _spriteBatch;

        [DependencyResolved]
        private EffectStore EffectStore { get; set; }
        
        [DependencyResolved]
        private GraphicsDevice Device { get; set; }
        
        private Effect _effect;
        private int _radius;
        private float _amount;
        private float _sigma;
        private float[] _kernel;
        private Vector2[] _offsetsHoriz;
        private Vector2[] _offsetsVert;

        /// <summary>
        /// Returns the radius of the Gaussian blur filter kernel in pixels.
        /// </summary>
        public int Radius => _radius;

        /// <summary>
        /// Returns the blur amount. This value is used to calculate the
        /// Gaussian blur filter kernel's sigma value. Good values for this
        /// property are 2 and 3. 2 will give a more blurred result whilst 3
        /// will give a less blurred result with sharper details.
        /// </summary>
        public float Amount => _amount;

        /// <summary>
        /// Returns the Gaussian blur filter's standard deviation.
        /// </summary>
        public float Sigma => _sigma;

        /// <summary>
        /// Returns the Gaussian blur filter kernel matrix. Note that the
        /// kernel returned is for a 1D Gaussian blur filter kernel matrix
        /// intended to be used in a two pass Gaussian blur operation.
        /// </summary>
        public float[] Kernel => _kernel;

        /// <summary>
        /// Returns the texture offsets used for the horizontal Gaussian blur
        /// pass.
        /// </summary>
        public Vector2[] TextureOffsetsX => _offsetsHoriz;

        /// <summary>
        /// Returns the texture offsets used for the vertical Gaussian blur
        /// pass.
        /// </summary>
        public Vector2[] TextureOffsetsY => _offsetsVert;

        /// <summary>
        /// This overloaded constructor instructs the GaussianBlur class to
        /// load and use its GaussianBlur.fx effect file that implements the
        /// two pass Gaussian blur operation on the GPU. The effect file must
        /// be already bound to the asset name: 'Effects\GaussianBlur' or
        /// 'GaussianBlur'.
        /// </summary>
        public GaussianBlur(float strength, SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
            ComputeKernel(7, strength);
        }

        private bool _isLoaded;
        [LoadAsync]
        private void Load()
        {
            _effect = EffectStore.Get("Resources/Shaders/Blur");

            _isLoaded = true;
        }

        /// <summary>
        /// Calculates the Gaussian blur filter kernel. This implementation is
        /// ported from the original Java code appearing in chapter 16 of
        /// "Filthy Rich Clients: Developing Animated and Graphical Effects for
        /// Desktop Java".
        /// </summary>
        /// <param name="blurRadius">The blur radius in pixels.</param>
        /// <param name="blurAmount">Used to calculate sigma.</param>
        public void ComputeKernel(int blurRadius, float blurAmount)
        {
            _radius = blurRadius;
            _amount = blurAmount;

            _kernel = null;
            _kernel = new float[_radius * 2 + 1];
            _sigma = _radius / _amount;

            var twoSigmaSquare = 2.0f * _sigma * _sigma;
            var sigmaRoot = (float)Math.Sqrt(twoSigmaSquare * Math.PI);
            var total = 0.0f;
            float distance;
            int index;

            for (var i = -_radius; i <= _radius; ++i)
            {
                distance = i * i;
                index = i + _radius;
                _kernel[index] = (float)Math.Exp(-distance / twoSigmaSquare) / sigmaRoot;
                total += _kernel[index];
            }

            for (var i = 0; i < _kernel.Length; ++i)
                _kernel[i] /= total;
        }

        /// <summary>
        /// Calculates the texture coordinate offsets corresponding to the
        /// calculated Gaussian blur filter kernel. Each of these offset values
        /// are added to the current pixel's texture coordinates in order to
        /// obtain the neighboring texture coordinates that are affected by the
        /// Gaussian blur filter kernel. This implementation has been adapted
        /// from chapter 17 of "Filthy Rich Clients: Developing Animated and
        /// Graphical Effects for Desktop Java".
        /// </summary>
        /// <param name="textureWidth">The texture width in pixels.</param>
        /// <param name="textureHeight">The texture height in pixels.</param>
        public void ComputeOffsets(float textureWidth, float textureHeight)
        {
            _offsetsHoriz = null;
            _offsetsHoriz = new Vector2[_radius * 2 + 1];

            _offsetsVert = null;
            _offsetsVert = new Vector2[_radius * 2 + 1];

            int index;
            var xOffset = 1.0f / textureWidth;
            var yOffset = 1.0f / textureHeight;

            for (var i = -_radius; i <= _radius; ++i)
            {
                index = i + _radius;
                _offsetsHoriz[index] = new Vector2(i * xOffset, 0.0f);
                _offsetsVert[index] = new Vector2(0.0f, i * yOffset);
            }
        }

        /// <summary>
        /// Performs the Gaussian blur operation on the source texture image.
        /// The Gaussian blur is performed in two passes: a horizontal blur
        /// pass followed by a vertical blur pass. The output from the first
        /// pass is rendered to renderTarget1. The output from the second pass
        /// is rendered to renderTarget2. The dimensions of the blurred texture
        /// is therefore equal to the dimensions of renderTarget2.
        /// </summary>
        /// <param name="srcTexture">The source image to blur.</param>
        /// <returns>The resulting Gaussian blurred image.</returns>
        public Texture2D PerformGaussianBlur(Texture2D srcTexture)
        {
            if (!_isLoaded)
                return null; // Not loaded yet.
            
            if (_effect == null)
                throw new InvalidOperationException("GaussianBlur.fx effect not loaded.");

            var renderTargetWidth = srcTexture.Width;
            var renderTargetHeight = srcTexture.Height;

            var renderTarget1 = new RenderTarget2D(Device, renderTargetWidth, renderTargetHeight, false,
                Device.PresentationParameters.BackBufferFormat, DepthFormat.None);

            var renderTarget2 = new RenderTarget2D(Device, renderTargetWidth, renderTargetHeight, false,
                Device.PresentationParameters.BackBufferFormat,
                DepthFormat.None);

            ComputeOffsets(renderTargetWidth, renderTargetHeight);

            Texture2D outputTexture;
            var destRect1 = new Rectangle(0, 0, renderTarget1.Width, renderTarget1.Height);
            var destRect2 = new Rectangle(0, 0, renderTarget2.Width, renderTarget2.Height);

            // Perform horizontal Gaussian blur.

            Device.SetRenderTarget(renderTarget1);

            _effect.CurrentTechnique = _effect.Techniques["GaussianBlur"];
            _effect.Parameters["weights"].SetValue(_kernel);
            _effect.Parameters["colorMapTexture"].SetValue(srcTexture);
            _effect.Parameters["offsets"].SetValue(_offsetsHoriz);

            _spriteBatch.Begin(0, BlendState.Opaque, null, null, null, _effect);
            _spriteBatch.Draw(srcTexture, destRect1, Color.White);
            _spriteBatch.End();

            // Perform vertical Gaussian blur.

            Device.SetRenderTarget(renderTarget2);
            outputTexture = renderTarget1;

            _effect.Parameters["colorMapTexture"].SetValue(outputTexture);
            _effect.Parameters["offsets"].SetValue(_offsetsVert);

            _spriteBatch.Begin(0, BlendState.Opaque, null, null, null, _effect);
            _spriteBatch.Draw(outputTexture, destRect2, Color.White);
            _spriteBatch.End();

            // Return the Gaussian blurred texture.

            Device.SetRenderTarget(null);
            outputTexture = renderTarget2;

            renderTarget1.Dispose();

            return outputTexture;
        }
    }
}
