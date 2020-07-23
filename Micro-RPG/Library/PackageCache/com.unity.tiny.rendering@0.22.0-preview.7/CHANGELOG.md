# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [0.22.0] - 2020-02-05

* Update package dependencies
* Fix crash on Mac il2cpp builds during Metal/OpenGL initialization.
* Add new tiny rendering settings build setting component
* `TinyDisplayInfo` is now exported via a configuration system
* Add custom inspectors for new light components
* Fix srgb color conversion to better match the editor scene view
* Runtime colorspace and sRGB usage will match Project Player colorspace settings

## [0.21.0] - 2020-01-21

* Add support for cascade shadow maps (1 csm directional light, fixed to four cascades). Refer to the CascadeShadowmappedLight component for more information.
* Add support for spot light inner angle.
* Fix culling under non-uniform scale when CompositeScale is used
* Update package dependencies

## [0.20.0] - 2019-12-10

* Update the package to use Unity '2019.3.0f1' or later
* This is the first release of Project Tiny lightweight rendering package.
