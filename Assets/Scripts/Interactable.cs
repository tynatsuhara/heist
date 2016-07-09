using System;

public interface Interactable {

	// Interact is called repeatedly. If you only want to use it
	// as a toggle, you can call InteractCancel on the character.
	void Interact(Character character);

	void Cancel(Character character);
}
