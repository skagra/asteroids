# Asteroids

A clone of the classic Asteroids arcade game using modern development tools.

Having recently written a version of the original Taito Space Invaders in raw Z80 machine code for the ZX Spectrum [here](https://github.com/skagra/space-invaders), 
I thought it would be interesting to recreate a classic arcade game using modern developments tools - in this case [Unity](https://unity.com/).

Core goals are:

* To learn Unity - I am totally new to Unity (though at least I have a good deal of experience with C#, which is used as the Unity scripting language).
* To accurately reproduce the game play experience of the orginal arcade machine, in look and feel.
* To attempt to mimic the vector display characteristics of the original arcade machine.

# Status

The development is mid way through with many of the major game play elements now in place.

![Asteroids](./docs/animation.gif)

* *Player ship* - With physics to match the original arcade game. Thrust animation and explosion animation. 
* *Player missiles*
* *Asteroids* - Three sizes of three shapes of asteroids moving at variable speeds and directions.  Large and medium steroids split when hit by either the ship of or a missile (small asteroids as destroyed).
* *UI* - Display of score and remaining lives and extra lives are awarded at defined score boundaries.
* *Sound* - Full sound using samples from the original arcade machine.

# References

* *Audio Samples* - Are taken from the [Classic Gaming](https://classicgaming.cc/classics/asteroids/sounds) site.
* *Sprites* - The sprites currently in use are by Joe Strout and were retrieved from [Open Game Art](https://opengameart.org/content/asteroids-vector-style-sprites).  I say midway as it is likely I'll switch them out for a version without the "glow" effect.
* An article about the Asteroids arcade machine at the [Retrogame Deconstruction Zone](https://www.retrogamedeconstructionzone.com/2019/10/asteroids-by-numbers.html).
* [Classic Gaming](https://classicgaming.cc/classics/asteroids/)
* [ROM Disassembly](https://www.6502disassembly.com/va-asteroids/)
* [ROM Disassembly](https://nmikstas.github.io/portfolio/asteroidsDisassembly/asteroidsDisassembly.html)
* [Computer Archeology](https://computerarcheology.com/Arcade/Asteroids/)
* [Asteroids HDL](https://nmikstas.github.io/portfolio/asteroidsHDL/asteroidsHDL.html)
* [The Arcade Blogger](https://arcadeblogger.com/2018/10/24/atari-asteroids-creating-a-vector-arcade-classic/)
   