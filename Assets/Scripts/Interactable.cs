using System;

/**
 * Something which is interactable is something which can be used
 * by a character. Basically, anything the player can walk up to
 * and press 'use'. Many things which are interactable are also
 * powerable.
 */
public interface Interactable {

	void Interact(Character character);
	void Uninteract(Character character);
}
