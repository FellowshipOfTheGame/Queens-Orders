// Fill out your copyright notice in the Description page of Project Settings.

#include "WarGame.h"
#include "RTS_Controller.h"

void ARTS_Controller::BeginPlay()
{
	Super::BeginPlay();

	bShowMouseCursor = true;
	bEnableClickEvents = true;
	bEnableMouseOverEvents = true;

}

void ARTS_Controller::setCurrentSelectedSlot(ABuildingSlot *slot)
{
	if (currentBuildingSlot == slot)
		return;


	// Hide options for that object
	if (currentBuildingSlot){
		currentBuildingSlot->HideOptions();
	}


	currentBuildingSlot = slot;

	// Show options for selected object
	if (currentBuildingSlot){
		currentBuildingSlot->ShowOptions();
	}
	
}


ABuildingSlot* ARTS_Controller::getCurrentSelectedSlot() const
{
	return currentBuildingSlot;
}