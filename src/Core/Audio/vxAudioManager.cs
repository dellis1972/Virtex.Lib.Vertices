#if VIRTICES_3D
#region Using Statements
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using vxVertices.Core.Entities;
using vxVertices.Core;


#endregion

namespace vxVertices.Audio
{
    /// <summary>
    /// Audio manager keeps track of what 3D sounds are playing, updating
    /// their settings as the camera and entities move around the world, and
    /// automatically disposing sound effect instances after they finish playing.
    /// </summary>
    public class vxAudioManager : Microsoft.Xna.Framework.GameComponent
    {
        #region Fields

        // The listener describes the ear which is hearing 3D sounds.
        // This is usually set to match the camera.
        public AudioListener Listener
        {
            get { return listener; }
        }

        AudioListener listener = new AudioListener();


        // The emitter describes an entity which is making a 3D sound.
        AudioEmitter emitter = new AudioEmitter();


        // Store all the sound effects that are available to be played.
        Dictionary<string, SoundEffect> soundEffects = new Dictionary<string, SoundEffect>();


        // Keep track of all the 3D sounds that are currently playing.
        public List<ActiveSound> activeSounds = new List<ActiveSound>();

        vxEngine vxEngine;

        #endregion


        public vxAudioManager(vxEngine vxEngine)
            : base(vxEngine.Game)
        {
            this.vxEngine = vxEngine;
        }


        /// <summary>
        /// Initializes the audio manager.
        /// </summary>
        public override void Initialize()
        {
            // Set the scale for 3D audio so it matches the scale of our game world.
            // DistanceScale controls how much sounds change volume as you move further away.
            // DopplerScale controls how much sounds change pitch as you move past them.
            SoundEffect.DistanceScale = 2000;
            SoundEffect.DopplerScale = 0.1f;
            SoundEffect.MasterVolume = 1;
            /*
            // Load all the sound effects.
            foreach (string soundName in soundNames)
            {
                soundEffects.Add(soundName, Game.Content.Load<SoundEffect>(soundName));
            }
            */
            base.Initialize();
        }


        /// <summary>
        /// Unloads the sound effect data.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    foreach (SoundEffect soundEffect in soundEffects.Values)
                    {
                        soundEffect.Dispose();
                    }

                    soundEffects.Clear();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public void Clear()
        {
            foreach (ActiveSound ac in activeSounds)
            {
                ac.Instance.Stop();
            }
        }


        /// <summary>
        /// Updates the state of the 3D audio system.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // Loop over all the currently playing 3D sounds.
            int index = 0;

            while (index < activeSounds.Count)
            {
                ActiveSound activeSound = activeSounds[index];

                if (activeSound.Instance.State == SoundState.Stopped)
                {
                    // If the sound has stopped playing, dispose it.
                    activeSound.Instance.Dispose();

                    // Remove it from the active list.
                    activeSounds.RemoveAt(index);
                }
                else
                {
                    // If the sound is still playing, update its 3D settings.
                    Apply3D(activeSound);

                    index++;
                }
            }

            base.Update(gameTime);
        }


        /// <summary>
        /// Triggers a new 3D sound Based off of Entity
        /// </summary>
		public SoundEffectInstance Play3DSound(SoundEffect soundEffect, bool isLooped, vxEntity3D emitter)
        {
            ActiveSound activeSound = new ActiveSound();

            // Fill in the instance and emitter fields.
            activeSound.Instance = soundEffect.CreateInstance();
            activeSound.Instance.IsLooped = isLooped;

            activeSound.Emitter = emitter;

            // Set the 3D position of this sound, and then play it.
            Apply3D(activeSound);

            activeSound.Instance.Volume = vxEngine.Profile.Settings.Audio.Double_SFX_Volume;

            activeSound.Instance.Play();


            // Remember that this sound is now active.
            activeSounds.Add(activeSound);

            return activeSound.Instance;
        }


        public SoundEffectInstance Play3DSound(SoundEffect soundEffect, bool isLooped, Vector3 position)
        {
            return Play3DSound(soundEffect, isLooped, position, 1);
        }

        /// <summary>
        /// Triggers a new 3D sound Based off of Entity Position.
        /// </summary>
        public SoundEffectInstance Play3DSound(SoundEffect soundEffect, bool isLooped, Vector3 position, float pitch)
        {
            ActiveSound activeSound = new ActiveSound();

            // Fill in the instance and emitter fields.
            activeSound.Instance = soundEffect.CreateInstance();
            activeSound.Instance.IsLooped = isLooped;

            //vxEntity emit = new vxEntity(position);
            //emit.World.Translation = position;
            activeSound.Emitter = new vxEntity3D(vxEngine, position/100);

            // Set the 3D position of this sound, and then play it.
            emitter.Position = activeSound.Emitter.World.Translation;
            emitter.Forward = activeSound.Emitter.World.Forward;
            emitter.Up = activeSound.Emitter.World.Up;
            emitter.Velocity = activeSound.Emitter.Velocity;
            
            activeSound.Instance.Apply3D(listener, emitter);
            
            activeSound.Instance.Volume = vxEngine.Profile.Settings.Audio.Double_SFX_Volume;

            activeSound.Instance.Play();

            // Remember that this sound is now active.
            activeSounds.Add(activeSound);

            return activeSound.Instance;
        }


        /// <summary>
        /// Updates the position and velocity settings of a 3D sound.
        /// </summary>
        private void Apply3D(ActiveSound activeSound)
        {
            emitter.Position = activeSound.Emitter.World.Translation;
            emitter.Forward = activeSound.Emitter.World.Forward;
            emitter.Up = activeSound.Emitter.World.Up;
            emitter.Velocity = activeSound.Emitter.Velocity;

            activeSound.Instance.Apply3D(listener, emitter);

        }


        /// <summary>
        /// Internal helper class for keeping track of an active 3D sound,
        /// and remembering which emitter object it is attached to.
        /// </summary>
        public class ActiveSound
        {
            public SoundEffectInstance Instance;
			public vxEntity3D Emitter;
        }
    }
}
#endif