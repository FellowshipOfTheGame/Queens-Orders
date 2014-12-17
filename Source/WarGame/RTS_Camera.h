// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "GameFramework/Pawn.h"
#include "RTS_Camera.generated.h"

/**
 * 
 */
UCLASS()
class WARGAME_API ARTS_Camera : public APawn
{
	GENERATED_BODY()
	
public:
	ARTS_Camera(const FObjectInitializer& ObjectInitializer);
	
	// Begin APawn override
	virtual void Tick(float DeltaSeconds) override;
	virtual void SetupPlayerInputComponent(UInputComponent* InputComponent) override;
	// End APawn override


	// Variables
	UPROPERTY(Category = Camera, VisibleDefaultsOnly, BlueprintReadOnly, meta = (AllowPrivateAccess = "true"))
	UCameraComponent *camera;


	// Static names for axis bindings
	static const FName MoveForwardBinding;
	static const FName MoveRightBinding;
};
